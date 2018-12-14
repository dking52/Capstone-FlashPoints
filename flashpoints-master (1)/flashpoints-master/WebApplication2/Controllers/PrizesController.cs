using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlashPoints.Data;
using FlashPoints.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlashPoints.Controllers
{
    [Authorize]
    public class PrizesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrizesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prizes
        public async Task<IActionResult> Index()
        {
            AddUserIfNotExists(User.Identity.Name);

            PrizeIndexViewModel mod = new PrizeIndexViewModel();

            mod.user = _context.User.Where(u => u.Email == User.Identity.Name)
                .Include(p => p.PrizesRedeemed)
                .First();

            mod.prizes = await _context.Prize
                .Include(p => p.PrizesRedeemed)
                .OrderBy(p => p.PointPrice)
                .ToListAsync();

            return View(mod);
        }

        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> ManagePrizes()
        {
            var prizes = await _context.Prize
                .ToListAsync();

            return View(prizes);
        }

        // GET: Prizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var prize = await _context.Prize
                .Include(p => p.PrizesRedeemed)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (prize == null)
            {
                return NotFound();
            }

            var user = _context.User.Where(u => u.Email == User.Identity.Name)
                .Include(p => p.PrizesRedeemed)
                .First();

            PrizeRedeemed rdm;

            try
            {
                rdm = user.PrizesRedeemed.Where(p => p.PrizeID == prize.ID).First();
            } catch
            {
                rdm = null;
            }

            if (rdm == null)
            {
                ViewBag.redeemed = false;
            } else
            {
                ViewBag.redeemed = true;
            }

            
            if (user.Points < prize.PointPrice)
            {
                ViewBag.redeemable = false;
            } else
            {
                ViewBag.redeemable = true;
            }

            ViewBag.userPoints = user.Points;

            return View(prize);
        }

        // POST: Prizes/RedeemPrize/5
        public async Task<IActionResult> RedeemPrize(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prize = await _context.Prize
                .FirstOrDefaultAsync(m => m.ID == id);
            if (prize == null)
            {
                return NotFound();
            }

            var user = _context.User.Where(u => u.Email == User.Identity.Name)
                .Include(u => u.PrizesRedeemed)
                .First();

            if (user.Points >= prize.PointPrice)
            {
                user.PrizesRedeemed.Add(new PrizeRedeemed
                {
                    User = user,
                    Prize = prize
                });
                _context.SaveChanges();
            }
            
            return RedirectToAction("Details", new { id = prize.ID });
        }

        // GET: Prizes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Description,PointPrice,ImagePath,CurrentInventory,Location,ActualCost")] Prize prize)
        {
            if (ModelState.IsValid)
            {
                prize.PrizesRedeemed = new List<PrizeRedeemed>();
                _context.Add(prize);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prize);
        }

        // GET: Prizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prize = await _context.Prize.FindAsync(id);
            if (prize == null)
            {
                return NotFound();
            }
            return View(prize);
        }

        // POST: Prizes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Description,PointPrice,ImagePath,CurrentInventory,Location,ActualCost")] Prize prize)
        {
            if (id != prize.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrizeExists(prize.ID))
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
            return View(prize);
        }

        // GET: Prizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prize = await _context.Prize
                .FirstOrDefaultAsync(m => m.ID == id);
            if (prize == null)
            {
                return NotFound();
            }

            return View(prize);
        }

        // POST: Prizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prize = await _context.Prize.FindAsync(id);
            _context.Prize.Remove(prize);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrizeExists(int id)
        {
            return _context.Prize.Any(e => e.ID == id);
        }

        public void AddUserIfNotExists(string email)
        {
            var query = _context.User.Where(e => e.Email == email);
            if (query.Count() == 0)
            {
                User newUser = new User();
                newUser.FirstName = User.FindFirst(ClaimTypes.GivenName).Value;
                newUser.LastName = User.FindFirst(ClaimTypes.Surname).Value;
                newUser.Email = email;
                newUser.PrizesRedeemed = new List<PrizeRedeemed>();
                newUser.EventsAttended = new List<EventAttended>();
                _context.User.Add(newUser);
                _context.SaveChanges();
            }
        }
    }
}
