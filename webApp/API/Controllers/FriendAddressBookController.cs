using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApp.Data;
using webApp.Models;
using webApp.Resources;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendAddressBookController : ControllerBase
    {
        private readonly MsSqlContext _context;
        private readonly IMapper _mapper;

        public FriendAddressBookController(MsSqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// The user with user id equal to userid is adding a user as a friend who has a user id equal to friendid
        /// </summary>
        /// <remarks>
        /// Both users need to add each other for a conversation to be allowed to happen
        /// </remarks>
        /// <response code="201">Successfully added the specified friend</response>
        /// <response code="400">If the user does not exist</response>
        /// <response code="404">If the friend does not exist</response>
        /// <response code="409">If the user has already added that friend</response>
        [HttpPost("{email}")]
        public async Task<ActionResult> AddFriend(string email)
        {
            var userId = Utils.GetCurrentUserId(this);
            User user = await _context.Users.FindAsync(userId);
            User friend = await _context.Users.Where(f => f.Email == email).FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            if (friend == null)
            {
                return NotFound("Friend does not exist.");
            }

            Friendship friendship = new Friendship
            {
                Friend = friend,
                User = user,
                FriendId = friend.UserId,
                UserId = userId
            };
            try
            {
                await _context.Friendships.AddAsync(friendship);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (FriendshipExists(userId, friend.UserId))
                {
                    return Conflict("User has already added this friend.");
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(201);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private bool FriendshipExists(string userId, string friendId)
        {
            return _context.Friendships.Any(e => ((e.UserId == userId) && (e.FriendId == friendId)));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool FriendsCanCommunicateSuccessfully(string userId, string friendId)
        {
            //Friends can communicate if they add each other as friends
            //Friends can communicate if neither users are blocked
            //Friends can communicate if they parents/child

            //if one the communication is done between a parent and a child then there must be one child of the user ids
            //the easiest way to check who the child is to try to obtain the child from the database
            Child childCheck = _context.Children.FindAsync(userId).Result;
            Child childCheck2 = _context.Children.FindAsync(friendId).Result;
            bool parentChildRelationship = false;
            bool friendshipExists = false;
            friendshipExists = _context.Friendships.Any(e => ((e.UserId == userId)
            && (e.FriendId == friendId)
            && (e.IsBlocked == false)))
            && _context.Friendships.Any(e => ((e.UserId == friendId)
            && (e.FriendId == userId)
            && (e.IsBlocked == false)));
            //if the first user is a child, then if the person with whom he wants to communicate is his parent, then first check will pass
            //then the users will be able to chat
            if (friendshipExists)
            {
                return true;
            }
            if (childCheck != null)
            {
                parentChildRelationship = childCheck.ParentId == friendId;
            }
            else if (childCheck2 != null)
            {
                parentChildRelationship = childCheck2.ParentId == userId;

            }
            return parentChildRelationship;

        }

        /// <summary>
        /// The user with user id equal to userid is removing an existing friend as a friend who has a user id equal to friendid
        /// </summary>
        /// <remarks>
        /// Both users need to add each other for a conversation to be allowed to happen
        /// </remarks>
        /// <response code="204">Successfully deleted the specified friend</response>
        /// <response code="400">If the user has not added that friend yet</response>
        [HttpDelete("{friendId}")]
        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            var userId = Utils.GetCurrentUserId(this);
            var friendship = await _context.Friendships.FindAsync(userId, friendId);
            if (friendship == null)
            {
                return BadRequest("User has not added this friend yet.");
            }

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// This retrieves the users that user with userid has added as a friend, even if these friends have not added them back
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<UserResource>> GetFriends()
        {
            var userId = Utils.GetCurrentUserId(this);
            List<Friendship> friendships = await _context.Friendships
            .Where(f => f.UserId == userId)
            .Include(f => f.Friend)
            .ToListAsync();

            var users = from f in friendships where f.Friend != null select f.Friend;

            var friendshipsResources = _mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);

            return friendshipsResources.ToList();
        }

        /// <summary>
        /// Fetch the friends of a specified child
        /// </summary>
        /// <param name="childId"></param>
        /// <response code="200">List of child's friends</response>
        /// <response code="401">User is not the parent of the child</response>
        /// <response code="404">Child does not exist</response>
        [HttpGet("{childId}")]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetChildFriends(string childId)
        {
            var parentId = Utils.GetCurrentUserId(this);
            Parent parent = await _context.Parents.FindAsync(parentId);
            Child child = await _context.Children.FindAsync(childId);

            if (child == null)
            {
                return NotFound();
            }

            if (child.ParentId != parentId)
            {
                return Unauthorized();
            }

            List<Friendship> friendships = await _context.Friendships
           .Where(f => f.UserId == childId)
           .Include(f => f.Friend)
           .ToListAsync();

            var users = from f in friendships where f.Friend != null select f.Friend;

            var friendshipsResources = _mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);

            return friendshipsResources.ToList();
        }
    }
}