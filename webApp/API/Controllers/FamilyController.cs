using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webApp.Data;
using webApp.Models;

namespace webApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyController : Controller
    {
        private readonly MsSqlContext _context;

        public FamilyController(MsSqlContext context)
        {
            _context = context;
        }

        //// POST: api/Family/5/6
        //[HttpPost("{parentId}/{childId}")]
        //public async Task<IActionResult> LinkChildAccount(string parentId, string childId)
        //{
        //    User parentUser = _context.Users.Find(parentId);
        //    User childUser = _context.Users.Find(childId);

        //    Parent parent = _context.Parents.Find(parentId);
        //    Child child = _context.Children.Find(childId);
        //    if (parentUser == null)
        //    {
        //        return BadRequest("Parent user does not exist.");
        //    }

        //    if (childUser != null)
        //    {
        //        if (parent != null)
        //        {
        //            if (child != null)
        //            {
        //                List<Child> children = (List<Child>)parent.Children;
        //                if (children.Contains(child))
        //                {
        //                    return Conflict("Parent is already linked to this child.");
        //                }

            //if (parentUser != null)
            //{
            //    if (childUser != null)
            //    {
            //        if (parent != null)
            //        {
            //            if (child != null)
            //            {
            //                List<Child> children = (List<Child>)parent.Children;
            //                if (children.Contains(child))
            //                {
            //                    return Conflict("Parent is already linked to this child.");
            //                }

        //                var parentsChildren = parent.Children;
        //                if (parentsChildren.Contains(child))
        //                {
        //                    return Conflict("Parent is already linked to this child.");
        //                }

                            //var parentsChildren = parent.Children;
                            //if (parentsChildren.Contains(child))
                            //{
                            //    return Conflict("Parent is already linked to this child.");
                            //}


        //                //friendship.IsBlocked = true;
        //                //_context.Entry(friendship).State = EntityState.Modified;

        //                try
        //                {
        //                    await _context.SaveChangesAsync();
        //                }
        //                catch (DbUpdateConcurrencyException)
        //                {
        //                    throw;
        //                }

        //                return StatusCode(201);
        //            }
        //            else
        //            {
        //                return BadRequest("Child does not exist.");
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("Parent does not exist.");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("Child user does not exist.");
        //    }

        //}

        // DELETE: api/Family/5/6
        //[HttpDelete("{parentId}/{childId}")]
        //public async Task<IActionResult> UnblockUser(string parentId, string childId)
        //{
        //    User user = _context.Users.Find(parentId);
        //    User friend = _context.Users.Find(childId);

        //    if (user != null)
        //    {
        //        if (friend != null)
        //        {
        //            var friendship = _context.Friendships.Find(parentId, childId);
        //            if (friendship == null)
        //            {
        //                return NotFound("The specified friendship does not exist.");
        //            }

        //            friendship.IsBlocked = false;
        //            _context.Entry(friendship).State = EntityState.Modified;

        //            try
        //            {
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                throw;
        //            }

        //            return NoContent();
        //        }
        //        else
        //        {
        //            return BadRequest("Friend does not exist.");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("User does not exist.");
        //    }
        //}   

        //// GET: Family
        //public async Task<IActionResult> Index()
        //{
        //    var msSqlContext = _context.Children.Include(c => c.Parent);
        //    return View(await msSqlContext.ToListAsync());
        //}

        //// GET: Family/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var child = await _context.Children
        //        .Include(c => c.Parent)
        //        .FirstOrDefaultAsync(m => m.UserId == id);
        //    if (child == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(child);
        //}

        //// GET: Family/Create
        //public IActionResult Create()
        //{
        //    ViewData["ParentId"] = new SelectList(_context.Parents, "UserId", "UserId");
        //    return View();
        //}

        //// POST: Family/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ParentId,UserId,Username,DateOfBirth,Email,Phone,Bio,CreatedAt,UpdatedAt")] Child child)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(child);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ParentId"] = new SelectList(_context.Parents, "UserId", "UserId", child.ParentId);
        //    return View(child);
        //}

        //// GET: Family/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var child = await _context.Children.FindAsync(id);
        //    if (child == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ParentId"] = new SelectList(_context.Parents, "UserId", "UserId", child.ParentId);
        //    return View(child);
        //}

        //// POST: Family/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("ParentId,UserId,Username,DateOfBirth,Email,Phone,Bio,CreatedAt,UpdatedAt")] Child child)
        //{
        //    if (id != child.UserId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(child);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ChildExists(child.UserId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ParentId"] = new SelectList(_context.Parents, "UserId", "UserId", child.ParentId);
        //    return View(child);
        //}

        //// GET: Family/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var child = await _context.Children
        //        .Include(c => c.Parent)
        //        .FirstOrDefaultAsync(m => m.UserId == id);
        //    if (child == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(child);
        //}

        //// POST: Family/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var child = await _context.Children.FindAsync(id);
        //    _context.Children.Remove(child);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ChildExists(string id)
        //{
        //    return _context.Children.Any(e => e.UserId == id);
        //}
    }
}
