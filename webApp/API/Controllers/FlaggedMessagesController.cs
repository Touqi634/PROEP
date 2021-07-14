using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webApp.Models;
using System.Linq;
using webApp.Data;
using AutoMapper;
using webApp.Resources;
using Microsoft.AspNetCore.Authorization;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlaggedMessagesController : ControllerBase
    {
        private readonly MsSqlContext _context;
        private readonly IMapper _mapper;

        public FlaggedMessagesController(MsSqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// This gets the messages that the user with user id equal to userid have as flagged
        /// <response code="400">If the user does not exist</response> 
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<FlaggedMessageResource>>> GetFlaggedMessages(string userId)
        {
            User user = await _context.Users.FindAsync(userId);
            var currentUserId = Utils.GetCurrentUserId(this);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            List<FlaggedMessage> flaggedMessages = await _context.FlaggedMessages
                .Where(fm => fm.SenderID == userId)
                .Include(fm => fm.Sender)
                .ToListAsync();

            var resources = _mapper.Map<IEnumerable<FlaggedMessage>, IEnumerable<FlaggedMessageResource>>(flaggedMessages);

            return resources.ToList();
        }

        /// <summary>
        /// This posts a flagged message
        /// <response code="400">If the flagged message to post is empty</response> 
        /// <response code="204">If the flagged message has been successfully posted</response>
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostFlaggedMessage(FlaggedMessage flaggedMessage)
        {
            if (flaggedMessage == null)
            {
                return BadRequest("Cannot add empty Flagged Message.");
            }

            await _context.FlaggedMessages.AddAsync(flaggedMessage);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
