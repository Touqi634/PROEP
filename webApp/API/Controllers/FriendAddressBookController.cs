using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApp.Data;
using webApp.Models;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendAddressBookController : ControllerBase
    {
        private readonly MsSqlContext _context;

        public FriendAddressBookController(MsSqlContext context)
        {
            _context = context;
        }

        // POST: api/FriendAddressBook/5/6
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Multiple parameters, see https://www.telerik.com/blogs/how-to-pass-multiple-parameters-get-method-aspnet-core-mvc
        [HttpPost("{userId}/{friendId}")]
        public async Task<ActionResult> AddFriend(string userId, string friendId)
        {
            User user = _context.Users.Find(userId);
            User friend = _context.Users.Find(friendId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            if (friend == null)
            {
                return NotFound("Friend does not exist.");
            }

            Friendship friendship = new Friendship{ Friend = friend, User = user, 
                FriendId = friendId, UserId = userId};
            

            try
            {
                _context.Friendships.Add(friendship);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FriendshipExists(userId, friendId))
                {
                    return Conflict("User has already added this friend.");
                }
                else
                {
                    throw;
                }
            }
            catch (InvalidOperationException)
            {
                if (FriendshipExists(userId, friendId))
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

        private bool FriendshipExists(string userId, string friendId)
        {
            return _context.Friendships.Any(e => ((e.UserId == userId) && (e.FriendId == friendId)));
        }

        // DELETE: api/FriendAddressBook/5/6
        [HttpDelete("{userId}/{friendId}")]
        public async Task<IActionResult> RemoveFriend(string userId, string friendId)
        {
            var friendship = _context.Friendships.Find(userId, friendId);
            if (friendship == null)
            {
                return BadRequest("User has not added this friend yet.");
            }

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/FriendAddressBook/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetFriends(string userId)
        {
            List<Friendship> friendships = await _context.Friendships
                .Where(f => f.UserId == userId)
                .Include(f => f.Friend)
                .ToListAsync();

            List<User> friends = new List<User>();
            // iterate through all friendships
            foreach (Friendship f in friendships)
            {
                if (f.Friend != null)
                {
                    friends.Add(f.Friend);
                }
            }

            return friends;
        }   

    }
}
