using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.DbContexts;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class CheckOutsController : Controller
    {
        private readonly LibraryDbContext _context;
        private static Random random = new Random();


        public CheckOutsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: CheckOuts
        public async Task<IActionResult> Index()
        {
            var table = _context.CheckOut.OrderBy(c => c.IsCheckedIn).ToList();
            //foreach (var item in table)
            //{
            //    if (DateTime.Now > item.ExpectedCheckInDate)
            //    {
            //        var book = await _context.Book.FirstOrDefaultAsync(c => c.BookId == item.BookId);
            //        item.AmountDefaulted = item.DefaultAmount;
            //        _context.Update(item);
            //        _context.SaveChanges();

            //    }

            //}
            return View(await _context.CheckOut.Include(c => c.Book).Include(c => c.Student).ToListAsync());


        }
        public IActionResult SearchRef()
        {
            var searchRefVM = new LibraryVM();
            return View(searchRefVM);
        }

        [HttpPost]
        public async Task<IActionResult> SearchRef(LibraryVM obj)
        {
            if (obj.CheckOutNotNull == null)
            {
                var checkout = await _context.CheckOut.Include(c => c.Book).Include( c=> c.Student).Where(m => m.CheckOutRef == obj.CheckOutRefNo).FirstOrDefaultAsync();
                // var checkout2 = _context.CheckOut.FirstOrDefault(m => m.CheckOutRef == obj.CheckOutRefNo);
                if (checkout == null)
                {
                    ModelState.AddModelError("", "");
                    return View(obj);
                }
                if (DateTime.Now > checkout.ExpectedCheckInDate)
                {
                   var defAmount = checkout.DefaultAmount;
                    var daysDefaulted = (DateTime.Now.Day - checkout.ExpectedCheckInDate.Day);
                    checkout.AmountDefaulted = defAmount * daysDefaulted;
                }
                obj.CheckOut = checkout;
                return View(obj);
            }
            else
            {
                var checkout = _context.CheckOut.Where(m => m.CheckOutRef == obj.CheckOutRefNo).FirstOrDefault();
                checkout.CheckInDate = DateTime.Now;


                checkout.IsCheckedIn = true;
                _context.CheckOut.Update(checkout);
                //await _context.SaveChangesAsync();

                var book = await _context.Book.FirstOrDefaultAsync(c => c.BookId == checkout.BookId);
                book.IsAvailable = true;
                _context.Book.Update(book);
                await _context.SaveChangesAsync();
                

               
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: CheckOuts/Details/5
        public async Task<IActionResult> CheckIn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkOut = await _context.CheckOut.Include(c => c.Book).Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.CheckOutId == id);
            if (checkOut == null)
            {
                return NotFound();
            }

            return View(checkOut);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(int? CheckOutId)
        {

            var checkOut = await _context.CheckOut.FirstOrDefaultAsync(m => m.CheckOutId == CheckOutId);
            if (checkOut == null)
            {
                return RedirectToAction("Details", "CheckOuts", new { failed = "yes", id = CheckOutId });
            }
            checkOut.CheckInDate = DateTime.Now;

            checkOut.IsCheckedIn = true;
            _context.Update(checkOut);
            await _context.SaveChangesAsync();

            var book = await _context.Book.FirstOrDefaultAsync(c => c.BookId == checkOut.BookId);
            book.IsAvailable = true;
            _context.Update(checkOut);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CheckOuts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CheckOuts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int BookId, string MatricNumber, DateTime ExpectedCheckInDate)
        {
            if (ModelState.IsValid)
            {
                var student = await _context.Students.FirstOrDefaultAsync(c => c.MatricNumber == MatricNumber);
                if (student == null)
                {
                    return RedirectToAction("Details", "Books", new { failed = "yes", id = BookId });
                }


                var book = await _context.Book.FirstOrDefaultAsync(c => c.BookId == BookId);
                var checkOutD = book.CheckoutDuration;

                var checkOut = new CheckOut
                {

                    BookId = BookId,
                    CheckOutDate = DateTime.Now,
                    CheckOutRef = RandomString(7),
                    ExpectedCheckInDate = DateTime.Now.AddDays(checkOutD),
                    DefaultAmount = book.DefaultAmount,
                    StudentId = student.StudentId,
                    IsCheckedIn = false
                };
                _context.CheckOut.Add(checkOut);
                await _context.SaveChangesAsync();

                book.IsAvailable = false;
                _context.Book.Update(book);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Details", "Books", new { failed = "yes" });
        }



        // GET: CheckOuts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkOut = await _context.CheckOut.FindAsync(id);
            if (checkOut == null)
            {
                return NotFound();
            }
            return View(checkOut);
        }

        // POST: CheckOuts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CheckOutId,BookId,StudentId,CheckOutDate,CheckInDate,AmountDefaulted")] CheckOut checkOut)
        {
            if (id != checkOut.CheckOutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkOut);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckOutExists(checkOut.CheckOutId))
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
            return View(checkOut);
        }

        // GET: CheckOuts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkOut = await _context.CheckOut
                .FirstOrDefaultAsync(m => m.CheckOutId == id);
            if (checkOut == null)
            {
                return NotFound();
            }

            return View(checkOut);
        }

        // POST: CheckOuts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkOut = await _context.CheckOut.FindAsync(id);
            _context.CheckOut.Remove(checkOut);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckOutExists(int id)
        {
            return _context.CheckOut.Any(e => e.CheckOutId == id);
        }



    }

}