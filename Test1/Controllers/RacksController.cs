using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Test1.Models;

namespace Test1.Controllers
{
    public class RacksController : Controller
    {
        private readonly TestContext _context;

        public RacksController(TestContext context)
        {
            _context = context;
        }

        // GET: Racks
        public async Task<IActionResult> Index()
        {
            //return _context.Racks != null ? 
            //            View(await _context.Racks.ToListAsync()) :
            //            Problem("Entity set 'TestContext.Racks'  is null.");

            var RakDetails = await _context.Racks.FromSqlRaw($"sp_GetRacks").ToListAsync();
            return View(RakDetails);
        }

        // GET: Racks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Racks == null)
            {
                return NotFound();
            }

            var rack = await _context.Racks
                .FirstOrDefaultAsync(m => m.RackId == id);
            if (rack == null)
            {
                return NotFound();
            }

            return View(rack);
        }

        // GET: Racks/Create
        public IActionResult Create()
        {
            ViewBag.data = _context.Racks.ToList();
            return View();
        }

        // POST: Racks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RackId,Code")] Rack rack)
        {
            if (ModelState.IsValid)
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@Code", rack.Code));
                var result = _context.Database.ExecuteSqlRaw(@"exec sp_insertRacksByID @Code", parameter.ToArray());
                return RedirectToAction("Index");
            }
            return View(rack);
        }

        // GET: Racks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Racks == null)
            {
                return NotFound();
            }

            var data = _context.Racks.FromSqlInterpolated($"exec sp_GetRacksByID {id};").ToList();
            ViewBag.data = _context.Racks.ToList();

            Rack r = new Rack { RackId = data[0].RackId, Code = data[0].Code };
            return View(r);
        }

        // POST: Racks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RackId,Code")] Rack rack)
        {
            if (id != rack.RackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var data = _context.Database.ExecuteSqlRaw($"exec sp_UpdateRacksByID {rack.RackId},{rack.Code}");
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RackExists(rack.RackId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rack);
        }

        // GET: Racks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Racks == null)
            {
                return NotFound();
            }

            var data = _context.Racks.FromSqlInterpolated($"exec sp_GetRacksByID {id};").ToList();
            ViewBag.data = _context.Racks.ToList();

            Rack r = new Rack { RackId = data[0].RackId, Code = data[0].Code };
            return View(r);
        }

        // POST: Racks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Racks == null)
            {
                return Problem("Entity set 'TestContext.Racks'  is null.");
            }
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@RackId", id));

            var result = _context.Database.ExecuteSqlRaw(@"exec sp_DELETERacksByID @RackId", parameter.ToArray());



            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RackExists(int id)
        {
          return (_context.Racks?.Any(e => e.RackId == id)).GetValueOrDefault();
        }
    }
}
