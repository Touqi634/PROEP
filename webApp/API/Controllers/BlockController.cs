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
    public class BlockController : ControllerBase
    {
        private readonly MsSqlContext _context;

        public BlockController(MsSqlContext context)
        {
            _context = context;
        }

        // GET: api/Block/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetBlockedUsers(string userId)
        {
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

        // POST: api/Block/5/6
        [HttpPost("{userId}/{friendId}")]
        public async Task<IActionResult> BlockUser(string userId, string friendId)
        {
            User user = _context.Users.Find(userId);
            User friend = _context.Users.Find(friendId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            if (friend == null)
            {
                return BadRequest("Friend does not exist.");
            }

            var friendship = _context.Friendships.Find(userId, friendId);
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
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(201);

        }

        // DELETE: api/Block/5/6
        [HttpDelete("{userId}/{friendId}")]
        public async Task<IActionResult> UnblockUser(string userId, string friendId)
        {
            User user = _context.Users.Find(userId);
            User friend = _context.Users.Find(friendId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            if (friend == null)
            {
                return BadRequest("Friend does not exist.");
            }

            var friendship = _context.Friendships.Find(userId, friendId);
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
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }
}
