using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Censored;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using webApp.API.Controllers;
using webApp.Data;
using webApp.Hubs;
using webApp.Models;

namespace webApp
{
    public interface IMessagesClient
    {
        Task ReceiveMessage(string senderID,string message);
        Task ReceiveError(string error);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class MessageHub : Hub<IMessagesClient>
    {
        private readonly MsSqlContext _context;
        private readonly IMapper mapper;
        private readonly Censor _censor;
        private readonly PackageFilter _packageFilter;
        public MessageHub(MsSqlContext context, IMapper mapper)
        {
            this._context = context;
            this.mapper = mapper;
            this._packageFilter = new PackageFilter("package1.txt");
            this._censor = new Censor(_packageFilter.GetCensoredWords());
        }
        public async Task<Task> SendMessageToUser(string receiverId,string messageText)
        {
            await using ( _context)
            {
                MessagesController controller = new MessagesController(_context);
                FlaggedMessagesController flaggedController = new FlaggedMessagesController(_context,mapper);
                UsersController usersController = new UsersController(_context);
                FriendAddressBookController friendAddressBookController = new FriendAddressBookController(_context,mapper);
                
                var message = new Message
                {
                    Status = MessageStatus.Sent,
                    Content = messageText,
                    SenderID = GetCurrentUserId(),
                    Sender = usersController.GetUser(GetCurrentUserId()).Result.Value,
                    ReceiverId = receiverId,
                    Receiver = usersController.GetUser(receiverId).Result.Value
                };
                if (!friendAddressBookController.FriendsCanCommunicateSuccessfully(message.SenderID, message.ReceiverId))
                {
                    return Clients.User(GetCurrentUserId()).ReceiveError("You are not a friend with the user or the friend is blocked");
                }
                controller.ControllerContext.HttpContext = Context.GetHttpContext();
                if (_censor.HasCensoredWord(messageText)) 
                {
                    await flaggedController.PostFlaggedMessage(new FlaggedMessage() { Content = messageText, Reason = "Swear Word", SenderID = message.SenderID, Sender = message.Sender });
                    return Clients.User(GetCurrentUserId()).ReceiveError("You sent an unpleasant phrase, this message will be reported");
                }
                await controller.PostMessage(message);
            }
            return Clients.User(receiverId).ReceiveMessage(GetCurrentUserId(), messageText);
        }
        private string GetCurrentUserId()
        {
            return Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}