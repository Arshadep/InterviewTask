using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Test1.Models;

namespace Test1.Controllers
{
    public class ShelvesController : Controller
    {
        private readonly TestContext _context;

        public ShelvesController(TestContext context)
        {
            _context = context;
        }

        // GET: Shelves
        public async Task<IActionResult> Index()
        {
              //return _context.Shelves != null ? 
              //            View(await _context.Shelves.ToListAsync()) :
              //            Problem("Entity set 'TestContext.Shelves'  is null.");

            var shelveDetails = await _context.Shelves.FromSqlRaw($"SP_GetShelves").ToListAsync();
            return View(shelveDetails);
        }

        // GET: Shelves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shelves == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelves
                .FirstOrDefaultAsync(m => m.ShelfId == id);
            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }

        // GET: Shelves/Create
        public IActionResult Create()
        {
            ViewBag.data = _context.Racks.ToList();
            return View();
        }

        // POST: Shelves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShelfId,Code,RackId")] Shelf shelf)
        {
            if (ModelState.IsValid)
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@Code", shelf.Code));
                parameter.Add(new SqlParameter("@RackID", shelf.RackId));
                var result = _context.Database.ExecuteSqlRaw(@"exec sp_InsertShelves @Code, @RackID", parameter.ToArray());


                //_context.Books.FromSqlInterpolated($"exec sp_InsertBookList {book.Code},{book.BookName},{book.Author},{book.Price}").ToList();

                return RedirectToAction("Index");
            }
            return View(shelf);
        }

        // GET: Shelves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shelves == null)
            {
                return NotFound();
            }

            //var shelf = await _context.Shelves.FindAsync(id);
            var data = _context.Shelves.FromSqlInterpolated($"exec SP_GetShelvesByID {id};").ToList();
            ViewBag.data = _context.Racks.ToList();

            if (data == null)
            {
                return NotFound();
            }

            Shelf s = new Shelf
            {
                ShelfId = data[0].ShelfId,
                Code = data[0].Code,
                RackId = data[0].RackId
            };
            return View(s);
        }

        // POST: Shelves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShelfId,Code,RackId")] Shelf shelf)
        {
            if (id != shelf.ShelfId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var data = _context.Database.ExecuteSqlRaw($"exec SP_UpdateshelvesByID {shelf.ShelfId},{shelf.Code},{shelf.RackId}");
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShelfExists(shelf.ShelfId))
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
            return View(shelf);
        }

        // GET: Shelves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shelves == null)
            {
                return NotFound();
            }

            var data = _context.Shelves.FromSqlInterpolated($"exec SP_GetShelvesByID {id};").ToList();
            ViewBag.data = _context.Racks.ToList();

            if (data == null)
            {
                return NotFound();
            }

            Shelf s = new Shelf
            {
                ShelfId = data[0].ShelfId,
                Code = data[0].Code,
                RackId = data[0].RackId
            };
            return View(s);

           // return View(shelf);
        }

        // POST: Shelves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shelves == null)
            {
                return Problem("Entity set 'TestContext.Shelves'  is null.");
            }
           // var shelf = await _context.Shelves.FindAsync(id);
            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@shelfId", id));

            var result = _context.Database.ExecuteSqlRaw(@"exec SP_DeleteshelvesByID @shelfId", parameter.ToArray());



            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShelfExists(int id)
        {
          return (_context.Shelves?.Any(e => e.ShelfId == id)).GetValueOrDefault();
        }
    }
}
