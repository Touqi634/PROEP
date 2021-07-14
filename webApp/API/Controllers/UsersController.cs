using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webApp.Models;
using webApp.Data;
using System;
using System.Security.Claims;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly MsSqlContext _context;

        public UsersController(MsSqlContext context)
        {
            _context = context;
        }

         // GET: api/Users
         /// <summary>
         /// Gets a list of all users.
         /// </summary>
         /// <remarks>
         ///    GET /api/Users
         /// </remarks>
         /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Edit a User.
        /// </summary>
        /// <remarks>
        ///     POST /api/Users/{id}
        ///     {
        ///         {user}
        ///     }
        /// </remarks>
        /// <param name="user"></param>
        /// <returns>No content</returns>
        /// <response code="201">Returns no content on successful change</response>
        /// <response code="404">If the user ID does not exist</response>
        /// <response code="400">If the user ID does not belong to the user to be changed</response>
        [HttpPut]
        public async Task<IActionResult> PutUser(User user)
        {
            var id = Utils.GetCurrentUserId(this);
            if (id != user.UserId)
            {
                return Unauthorized();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create a user of type Parent
        /// </summary>
        /// <remarks>
        ///     Post /api/Users
        ///     {
        ///         {user}
        ///     }
        /// </remarks>
        /// <param name="user"></param>
        /// <response code="201">User created</response>
        /// <response code="400">Validation errors</response>
        /// <response code="409">User already exists</response>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(Parent user)
        {
            var id = Utils.GetCurrentUserId(this);
            if (id != user.UserId)
            {
                return Unauthorized();
            }
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(user);

            if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                return BadRequest(validationResults);
            }
            
            await _context.Parents.AddAsync(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (UserExists(user.UserId))
                {
                    return Conflict();
                }

                throw;
            }

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
