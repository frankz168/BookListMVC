using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Book Book { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Book = new Book();
            if (id == null)
                //create
                return View(Book);
            //update
            Book = _db.Book.FirstOrDefault(m => m.Id == id);
            if (Book == null)
                return NotFound();

            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                    // insert query in DB
                    _db.Book.Add(Book);
                else
                    // update query in DB
                    _db.Book.Update(Book);

                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Book);
        }


        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetBookAll()
        {
            var test = await _db.Book.ToListAsync();
            return Json(new { data = await _db.Book.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDB = await _db.Book.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDB == null)
                return Json(new { success = false, message = "Error while Deleting" });

            _db.Book.Remove(bookFromDB);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successfully" });
        }

        #endregion
    }
}
