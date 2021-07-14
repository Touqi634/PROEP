using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webApp.Models;
using webApp.Data;
using System.Linq;
using System;

namespace webApp.API.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly MsSqlContext _context;
        public MessagesController(MsSqlContext context)
        {
            _context = context;
        }

        // GET: api/Messages/2
        [HttpGet("with/{id}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesWith(string id)
        {
            var userId = Utils.GetCurrentUserId(this);
            return await _context.Messages.Where((m)=>(m.SenderID==userId&&m.ReceiverId==id) || (m.SenderID == id  && m.ReceiverId == userId)).ToListAsync();
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var userId = Utils.GetCurrentUserId(this);
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            if (message.SenderID != userId || message.ReceiverId != userId)
            {
                return Unauthorized();
            }

            return message;
        }
        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            var userId = Utils.GetCurrentUserId(this);

            if (id != message.MessageId)
            {
                return BadRequest();
            }
            if (message.SenderID != userId)
            {
                return Unauthorized();
            }
            _context.Entry(message).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }

                throw;
            }
            return NoContent();
        }
        // POST: api/Messages
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            var userId = Utils.GetCurrentUserId(this);

            if (message.SenderID != userId)
            {
                return Unauthorized();
            }

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.MessageId }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var userId = Utils.GetCurrentUserId(this);
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            if (message.SenderID != userId)
            {
                return Unauthorized();
            }

                _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
