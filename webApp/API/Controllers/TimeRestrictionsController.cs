using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApp.Data;
using webApp.Models;
using webApp.Resources;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeRestrictionsController : ControllerBase
    {
        private readonly MsSqlContext _context;
        private readonly IMapper _mapper;

        public TimeRestrictionsController(MsSqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// This retrieves the time restrictions that child with childid has
        /// </summary>
        [HttpGet("{childId}")]
        public async Task<ActionResult<IEnumerable<TimeRestrictionResource>>> GetTimeRestrictions(string childId)
        {
            List<TimeRestriction> timeRestrictions = await _context.TimeRestrictions
                .Where(tr => (tr.RestrictedUserId == childId))
                .ToListAsync();

            // Mapping done here
            var resources = _mapper.Map<IEnumerable<TimeRestriction>, IEnumerable<TimeRestrictionResource>>(timeRestrictions);

            return resources.ToList();
        }

        /// <summary>
        /// This adds a time restriction for a specific child which is mentioned in the time restriction object itself
        /// </summary>
        /// <response code="201">Successfully added the time restriction</response>
        /// <response code="401">If user requesting the add is not the parent of the child restricted</response> 
        /// <response code="400">If the child does not exist</response> 
        [HttpPost]
        public async Task<IActionResult> AddTimeRestriction(TimeRestriction timeRestriction)
        {
            Child child = await _context.Children.FindAsync(timeRestriction.RestrictedUserId);
            var parentId = Utils.GetCurrentUserId(this);
            
            if (child == null)
            {
                return BadRequest("Child does not exist.");
            }
            if (child.ParentId != parentId)
            {
                return Unauthorized("Only the parent of a child can set time restrictions");
            }

            await _context.TimeRestrictions.AddAsync(timeRestriction);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return StatusCode(201);
        }

        /// <summary>
        /// This updates the time restriction with time restriction id equal to id
        /// </summary>
        /// <response code="204">Successfully updated the time restriction</response>
        /// <response code="400">If specified id does not match the id of the time restriction object</response>
        /// <response code="401">If user requesting the update is not the parent of the child restricted</response> 
        /// <response code="404">If a time restriction object with id equal to the specified id does not exist</response> 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeRestriction(int id, TimeRestriction timeRestriction)
        {
            if (id != timeRestriction.RestrictionId)
            {
                return BadRequest();
            }
            var parentId = Utils.GetCurrentUserId(this);
            if (timeRestriction.RestrictedUser.ParentId != parentId)
            {
                return Unauthorized();
            }

            _context.Entry(timeRestriction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!TimeRestrictionExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        private bool TimeRestrictionExists(int id)
        {
            return _context.TimeRestrictions.Any(e => e.RestrictionId == id);
        }

        /// <summary>
        /// This deletes the time restriction with time restriction id equal to timeRestrictionId
        /// </summary>
        /// <response code="204">Successfully deleted the time restriction</response>
        /// <response code="404">If a time restriction object with id equal to the specified id does not exist</response> 
        /// <response code="401">If user requesting the delete is not the parent of the child restricted</response> 
        [HttpDelete("{timeRestrictionId}")]
        public async Task<IActionResult> DeleteTimeRestriction(int timeRestrictionId)
        {
            TimeRestriction timeRestriction = await _context.TimeRestrictions
                .Include(tr => tr.RestrictedUser)
                .FirstOrDefaultAsync(tr => tr.RestrictionId == timeRestrictionId);

            if (timeRestriction == null)
            {
                return NotFound();
            }
            var parentId = Utils.GetCurrentUserId(this);
            if (timeRestriction.RestrictedUser.ParentId != parentId)
            {
                return Unauthorized();
            }
            _context.TimeRestrictions.Remove(timeRestriction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
