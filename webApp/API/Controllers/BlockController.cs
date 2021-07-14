using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApp.Data;
using webApp.Models;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlockController : ControllerBase
    {
        private readonly MsSqlContext _context;

        public BlockController(MsSqlContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This gets the users that the user with user id equal to userid have blocked
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetBlockedUsers()
        {
            var userId = Utils.GetCurrentUserId(this);
            List<Friendship> friendships = await _context.Friendships
                .Where(f => (f.IsBlocked) && (f.UserId == userId))
                .Include(f => f.Friend)
                .ToListAsync();

            List<User> blockedFriends = new List<User>();
            // iterate through all friendships in the database
            foreach (Friendship f in friendships)
            {
                if (f.Friend != null)
                {
                    blockedFriends.Add(f.Friend);
                }
            }

            return blockedFriends;
        }

        /// <summary>
        /// The user with user id equal to userid is attempting to block their friend with user id friendid
        /// </summary>
        /// <response code="201">Successfully blocked the specified friend</response>
        /// <response code="400">If the user or friend does not exist</response> 
        /// <response code="404">If the friendship does not exist</response> 
        /// <response code="409">If the user has already blocked that friend</response>
        [HttpPost("{friendId}")]
        public async Task<IActionResult> BlockUser(string friendId)
        {
            var userId = Utils.GetCurrentUserId(this);
            User user = await _context.Users.FindAsync(userId);
            User friend = await _context.Users.FindAsync(friendId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            if (friend == null)
            {
                return BadRequest("Friend does not exist.");
            }

            var friendship = await _context.Friendships.FindAsync(userId, friendId);
            if (friendship == null)
            {
                return NotFound("The specified friendship does not exist.");
            }

            if (friendship.IsBlocked)
            {
                return Conflict("User has already blocked this friend.");
            }

            friendship.IsBlocked = true;
            _context.Entry(friendship).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return StatusCode(201);

        }

        /// <summary>
        /// The user with user id equal to userid is attempting to unblock their previously blocked friend with user id friendid
        /// </summary>
        /// <response code="204">Successfully unblocked the specified friend</response>
        /// <response code="400">If the user or friend does not exist</response> 
        /// <response code="404">If the friendship does not exist</response> 
        /// <response code="409">If the user has not already blocked that friend</response> 
        [HttpDelete("{childId}/{friendId}")]
        public async Task<IActionResult> UnblockUser(string childId, string friendId)
        {
            
            var userId = Utils.GetCurrentUserId(this);
            User user = await _context.Users.FindAsync(userId);
            User friend = await _context.Users.FindAsync(friendId);
            Child child = await _context.Children.FindAsync(childId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            if (userId != child.ParentId)
            {
                return Unauthorized("Only the parent can unblock friend");
            }
            if (friend == null)
            {
                return BadRequest("Friend does not exist.");
            }
            var friendship = await _context.Friendships.FindAsync(childId, friendId);
            if (friendship == null)
            {
                return NotFound("The specified friendship does not exist.");
            }

            if (!friendship.IsBlocked)
            {
                return Conflict("User has not blocked this friend.");
            }

            friendship.IsBlocked = false;
            _context.Entry(friendship).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return NoContent();
        }

        [HttpPost("{childId}/{friendId}")]
        public async Task<IActionResult> BlockUser(string childId, string friendId)
        {

            var userId = Utils.GetCurrentUserId(this);
            User user = await _context.Users.FindAsync(userId);
            User friend = await _context.Users.FindAsync(friendId);
            Child child = await _context.Children.FindAsync(childId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            if (userId != child.ParentId)
            {
                return Unauthorized("Only the parent can Block friend of this child");
            }
            if (friend == null)
            {
                return BadRequest("Friend does not exist.");
            }

            Friendship friendship = await _context.Friendships.FirstOrDefaultAsync(p => p.UserId == childId && p.FriendId == friendId);
            //var friendship = await _context.Friendships.FindAsync(userId, friendId);
            if (friendship == null)
            {
                return NotFound("The specified friendship does not exist.");
            }

            if (friendship.IsBlocked)
            {
                return Conflict("User has already blocked this friend.");
            }

            friendship.IsBlocked = true;
            _context.Entry(friendship).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return StatusCode(201);
        }
    }
}
