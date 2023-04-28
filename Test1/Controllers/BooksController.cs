using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Test1.Models;
using Test1.ViewModel;
using iTextSharp.tool.xml;

namespace Test1.Controllers
{
    public class BooksController : Controller
    {
        private readonly TestContext _context;

        public BooksController(TestContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            //return _context.Books != null ? 
            //            View(await _context.Books.ToListAsync()) :
            //            Problem("Entity set 'TestContext.Books'  is null.");
            var bookDetails = await _context.Books.FromSqlRaw($"sp_GetBookDetails").ToListAsync();
            return View(bookDetails);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            ViewBag.data = _context.Shelves.ToList();


            //return  _context.Books.FromSqlRaw("EXECUTE dbo.sp_Visits @Id", parameters).FirstOrDefault();


            var data = _context.Books.FromSqlInterpolated($"exec sp_GetBookDetailsById {id};").ToList();


            //var data =  _context.Books.FromSqlRaw($"sp_GetBookDetailsById {id} ").FirstOrDefault();

            BooksViewModel bm = new BooksViewModel
            {
                Id = data[0].Id,
                Code = data[0].Code,
                BookName = data[0].BookName,
                Author = data[0].Author,
                Price = data[0].Price,
                IsAvailable = data[0].IsAvailable,
                Status = (string)data[0].Status,
                ShelfId = data[0].ShelfId
            };
            return View(bm);
        }

        [HttpPost]
        public FileResult ExportExcel(string ExportData)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader reader = new StringReader(ExportData);
                Document PdfFile = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                PdfFile.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
                PdfFile.Close();
                
                return File(stream.ToArray(), "application/pdf", "ExportData.pdf");
            }
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewBag.data = _context.Shelves.ToList();
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,BookName,Author,IsAvailable,Price,ShelfId,IsDelete")] BooksViewModel book)
        {
            if (ModelState.IsValid)
            {

                //var data = _context.Books.FromSqlInterpolated($"exec sp_InsertBookList {book.Code},{book.BookName},{book.Author},{book.Price}");
                //return RedirectToAction("Index");

                //return RedirectToAction(nameof(Index));
                bool isactive = false;
                bool isstatus = false;

                string d = book.Status;

                int dt = Convert.ToInt16(book.IsAvailable);

                int st = Convert.ToInt16(book.Status);

                if (dt==1)
                {
                    isactive = true;
                }
                if (st ==1)
                {
                    isstatus = true;
                }

                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@Code", book.Code));
                parameter.Add(new SqlParameter("@BookName", book.BookName));
                parameter.Add(new SqlParameter("@Auther", book.Author));
                parameter.Add(new SqlParameter("@price", book.Price));
                parameter.Add(new SqlParameter("@IsAvailable", isactive));
                parameter.Add(new SqlParameter("@ShelfId", book.ShelfId));
                var result =  _context.Database.ExecuteSqlRaw(@"exec sp_InsertBookList @Code, @BookName, @Auther, @IsAvailable,@price,@ShelfId", parameter.ToArray());


                //_context.Books.FromSqlInterpolated($"exec sp_InsertBookList {book.Code},{book.BookName},{book.Author},{book.Price}").ToList();
                
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }


            ViewBag.data = _context.Shelves.ToList();


            string[] items = { "Yes", "No" };
            ViewBag.Isavaliable = items;

            //return  _context.Books.FromSqlRaw("EXECUTE dbo.sp_Visits @Id", parameters).FirstOrDefault();


            var data = _context.Books.FromSqlInterpolated($"exec sp_GetBookDetailsById {id};").ToList();


            //var data =  _context.Books.FromSqlRaw($"sp_GetBookDetailsById {id} ").FirstOrDefault();

            BooksViewModel bm = new BooksViewModel
            {
                Id = data[0].Id,
                Code = data[0].Code,
                BookName = data[0].BookName,
                Author = data[0].Author,
                Price = data[0].Price,
                IsAvailable = data[0].IsAvailable,
                Status = (string)data[0].Status,
                ShelfId= data[0].ShelfId
            };

            //var book = await _context.Books.FindAsync(id);
            //if (book == null)
            //{
            //    return NotFound();
            //}
            return View(bm);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,BookName,Author,IsAvailable,Price,ShelfId,IsDelete")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool isAvial = false;

                    var dt = book.IsAvailable.ToUpper();
                    if (dt=="YES")
                    {
                        isAvial = true;
                    }
                    var data = _context.Database.ExecuteSqlRaw($"exec sp_UpdateDookList {book.Id},{book.Code},{book.BookName},{book.Author},{isAvial},{book.Price}");
                    return RedirectToAction("Index");

                    //                    var data = await  _context.Books.ExecuteUpdateAsync($"sp_UpdateDookList {id},{book.Code},{book.BookName},{book.Author},{book.IsAvailable},{book.Price}");
                    // _context.Update(book);
                    // await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }


            var data = _context.Books.FromSqlInterpolated($"exec sp_GetBookDetailsById {id};").ToList();


            //var data =  _context.Books.FromSqlRaw($"sp_GetBookDetailsById {id} ").FirstOrDefault();

          

            //var book = await _context.Books
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (bm == null)
            //{
            //    return NotFound();
            //}


            BooksViewModel bm = new BooksViewModel
            {
                Id = data[0].Id,
                Code = data[0].Code,
                BookName = data[0].BookName,
                Author = data[0].Author,
                Price = data[0].Price,
                IsAvailable = data[0].IsAvailable,
                Status = (string)data[0].Status,
                ShelfId= data[0].ShelfId
            };

            return View(bm);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'TestContext.Books'  is null.");
            }
            //var book = await _context.Books.FindAsync(id);
            //if (book != null)
            //{

            //    var parameter = new List<SqlParameter>();
            //    parameter.Add(new SqlParameter("@Id", book.Id));

            //    var result = _context.Database.ExecuteSqlRaw(@"exec sp_DeleteBookList @Id", parameter.ToArray());


            //  //  _context.Books.Remove(book);
            //}

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@Id", id));

            var result = _context.Database.ExecuteSqlRaw(@"exec sp_DeleteBookList @Id", parameter.ToArray());



            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
