using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webApp.Resources;
using webApp.Models;
using webApp.Data;
using System.Linq;
using AutoMapper;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FamilyController : ControllerBase
    {
        private readonly MsSqlContext _context;
        private readonly IMapper _mapper;

        public FamilyController(MsSqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/Family/5/children
        /// <summary>
        /// Get a list of all children belonging to a parent.
        /// </summary>
        /// <remarks>
        ///     Get /api/Family/{parentId}/children
        /// </remarks>
        /// <response code="200">List of children returned</response>
        /// <response code="404">Parent does not exist</response>
        [HttpGet("children")]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetChildren()
        {
            var parentId = Utils.GetCurrentUserId(this);

            Parent parent = await _context.Parents.Include(p => p.Children).FirstOrDefaultAsync(p => p.UserId == parentId);


            if (parent == null)
            {
                return NotFound();
            }

            var resources = _mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(parent.Children);

            return resources.ToList();
        }
        // Get: api/Family/parent
        /// <summary>
        /// Get the parent of a child.
        /// </summary>
        /// <remarks>
        ///     Get /api/Family/parent
        /// </remarks>
        /// <response code="200">Parent of child returned</response>
        /// <response code="404">Child does not exist</response>
        [HttpGet("parent")]
        public async Task<ActionResult<UserResource>> GetParent()
        {
            var childId = Utils.GetCurrentUserId(this);

            Child child = await _context.Children.Include(c => c.Parent).FirstOrDefaultAsync(c => c.UserId == childId);

            if (child == null)
            {
                return NotFound();
            }

            var resource = _mapper.Map<UserResource>(child.Parent);

             return resource;
    
        }
        
        // POST: api/Family/5
        /// <summary>
        /// Create a child belonging to the parent.
        /// </summary>
        /// <remarks>
        ///     Post /api/Family/
        ///     {
        ///         {Child}
        ///     }
        /// </remarks>
        /// <param name="child"></param>
        /// <response code="201">Child created.</response>
        /// <response code="404">Parent does not exist.</response>
        /// <response code="400">Specified parent of child does not match parent.</response>
        /// <response code="400">Validation errors.</response>
        /// <response code="409">Child already exists.</response>
        [HttpPost]
        public async Task<IActionResult> CreateChild(Child child)
        {
            var parentId = Utils.GetCurrentUserId(this);
            Parent parent = await _context.Parents.FindAsync(parentId);

            if (parent == null)
            {
                return NotFound("Parent does not exist.");
            }

            if (child.ParentId != parentId )
            {
                return Unauthorized("Child needs to belong to the parent");
            }

            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(child);

            if (!Validator.TryValidateObject(child, validationContext, validationResults, true))
            {
                return BadRequest(validationResults);
            }

            await _context.Children.AddAsync(child);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (_context.Children.Any(e => e.UserId == child.UserId))
                {
                    return Conflict();
                }

                throw;
            }

            return NoContent();
        }
        
        ///DELETE: api/Family/5/6
        /// <summary>
        /// Delete a child.
        /// </summary>
        /// <remarks>
        ///     Delete /api/Family/{parentId}/{ChildId}
        /// </remarks>
        /// <param name="childId"></param>
        /// <response code="201">Child was deleted</response>
        /// <response code="404">Child not found</response>
        [HttpDelete("{childId}")]
        public async Task<IActionResult> DeleteChild(string childId)
        {
            var parentId = Utils.GetCurrentUserId(this);
            Parent parent = await _context.Parents.FindAsync(parentId);
            Child child = await _context.Children.FindAsync(childId);

            if (parent == null || child == null)
            {
                return NotFound();
            }
            if (child.ParentId != parentId)
            {
                return Unauthorized();
            }
            _context.Users.Remove(child);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// View chat logs of child with friend
        /// </summary>
        /// <param name="childId"></param>
        /// <param name="friendId"></param>
        /// <response code="200">Chat logs of child with friend</response>
        /// <response code="401">Child is not a child of current user</response>
        /// <response code="404">Child or friend does not exist</response>
        [HttpGet("{childId}/chats/{friendId}")]
        public async Task<ActionResult<IEnumerable<MessageResource>>> GetChildChatLogs(string childId, string friendId)
        {
            var parentId = Utils.GetCurrentUserId(this);
            Parent parent = await _context.Parents.FindAsync(parentId);
            Child child = await _context.Children.FindAsync(childId);
            User friend = await _context.Users.FindAsync(friendId);

            if (child == null || friend == null)
            {
                return NotFound();
            }
            if (child.ParentId != parentId)
            {
                return Unauthorized();
            }

            var messages = _context.Messages
                .AsQueryable()
                .Where(m => 
                    (m.SenderID == childId && m.ReceiverId == friendId) ||
                    (m.SenderID == friendId && m.ReceiverId == childId)
                ).ToList();

            var resources = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageResource>>(messages).ToList();

            return resources;
        }
    }
}
