using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoXpress3.Data;
using MotoXpress3.Models;

namespace MotoXpress3.Controllers
{
    public class RentalRecordsController : Controller
    {
        private readonly MotoXpressDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RentalRecordsController(MotoXpressDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        //GET: RentalRecords
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            Guid userGuid;
            if (!Guid.TryParse(userId, out userGuid))
            {
                return Unauthorized();
            }

            var rentalRecords = await _context.RentalRecords
            .Include(r => r.Bike)
            .Include(r => r.User)
            .Where(r => isAdmin || r.UserId.ToString() == userId)
            .ToListAsync();

            return View(rentalRecords);
        }

        // GET: RentalRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalRecord = await _context.RentalRecords
                .Include(r => r.Bike)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rentalRecord == null)
            {
                return NotFound();
            }

            return View(rentalRecord);
        }

        // GET: RentalRecords/Create
        public IActionResult Create()
        {
            ViewData["BikeId"] = new SelectList(_context.Bikes, "BikeId", "BikeName");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: RentalRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,BikeId,UserId,RentalDate,ReturnDate")] RentalRecord rentalRecord)
        {
            if (ModelState.IsValid)
            {
                var bike = await _context.Bikes.FindAsync(rentalRecord.BikeId);
                if (bike == null)
                {
                    return NotFound();
                }

                rentalRecord.TotalCost = (rentalRecord.ReturnDate - rentalRecord.RentalDate).Days * bike.PerDayRental;
                _context.Add(rentalRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BikeId"] = new SelectList(_context.Bikes, "BikeId", "BikeName", rentalRecord.BikeId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", rentalRecord.UserId);
            return View(rentalRecord);
        }

        // GET: RentalRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalRecord = await _context.RentalRecords.FindAsync(id);
            if (rentalRecord == null)
            {
                return NotFound();
            }
            ViewData["BikeId"] = new SelectList(_context.Bikes, "BikeId", "BikeId", rentalRecord.BikeId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", rentalRecord.UserId);
            return View(rentalRecord);
        }

        // POST: RentalRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,BikeId,UserId,RentalDate,ReturnDate,TotalCost")] RentalRecord rentalRecord)
        {
            if (id != rentalRecord.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.RentalRecords.Update(rentalRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalRecordExists(rentalRecord.RentalId))
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
            ViewData["BikeId"] = new SelectList(_context.Bikes, "BikeId", "BikeId", rentalRecord.BikeId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", rentalRecord.UserId);
            return View(rentalRecord);
        }

        // GET: RentalRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalRecord = await _context.RentalRecords
                .Include(r => r.Bike)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rentalRecord == null)
            {
                return NotFound();
            }

            return View(rentalRecord);
        }

        // POST: RentalRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rentalRecord = await _context.RentalRecords.FindAsync(id);
            if (rentalRecord != null)
            {
                _context.RentalRecords.Remove(rentalRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalRecordExists(int id)
        {
            return _context.RentalRecords.Any(e => e.RentalId == id);
        }

        //// GET: RentalRecords/RentBike
        //public IActionResult RentBike()
        //{
        //    ViewData["BikeId"] = new SelectList(_context.Bikes, "BikeId", "BikeName");
        //    ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
        //    return View();
        //}

        //// POST: RentalRecords/RentBike
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RentBike([Bind("RentalId,BikeId,UserId,RentalDate,ReturnDate")] RentalRecord rentalRecord)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var bike = await _context.Bikes.FindAsync(rentalRecord.BikeId);
        //        if (bike == null)
        //        {
        //            return NotFound();
        //        }

        //        rentalRecord.TotalCost = (rentalRecord.ReturnDate - rentalRecord.RentalDate).Days * bike.PerDayRental;
        //        _context.Add(rentalRecord);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BikeId"] = new SelectList(_context.Bikes, "BikeId", "BikeName", rentalRecord.BikeId);
        //    ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", rentalRecord.UserId);
        //    return View(rentalRecord);
        //}

        // GET: RentalRecords/RentBike
        public IActionResult RentBike()
        {
            ViewData["BikeName"] = new SelectList(_context.Bikes, "BikeId", "BikeName");
            ViewData["BikeImage"] = new SelectList(_context.Bikes, "BikeId", "BikeImage");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName");
            return View();
        }


        // POST: RentalRecords/RentBike
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentBike([Bind("RentalId,BikeId,BikeImage,UserId,RentalDate,ReturnDate")] RentalRecord rentalRecord)
        {
            // Get the current logged-in user's email from Identity
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var userEmail = user.Email;
            if (ModelState.IsValid)
            {
                var bike = await _context.Bikes.FindAsync(rentalRecord.BikeId);
                if (bike == null)
                {
                    return NotFound();
                }
                // Assign the email to the User field in RentalRecord
                rentalRecord.UserId = userId;

                rentalRecord.TotalCost = (rentalRecord.ReturnDate - rentalRecord.RentalDate).Days * bike.PerDayRental;
                _context.RentalRecords.Add(rentalRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BikeId"] = new SelectList(_context.Bikes, "BikeId", "BikeName", rentalRecord.BikeId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", rentalRecord.UserId);
            return View(rentalRecord);
        }

    }
}
