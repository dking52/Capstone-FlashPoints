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
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Index(string searchString, string currentFilter)
        {
            AddUserIfNotExists(User.Identity.Name);

            if (searchString != null)
            {
                ViewBag.SearchString = searchString;
            }

            ViewBag.currentFilter = currentFilter;

            if (!String.IsNullOrEmpty(searchString))
            {
                // Query the database using the search parameter.
                var userSearch = (from u in _context.User
                                  where
                                    u.Email.Contains(searchString) // search by email 
                                     || u.FirstName.Contains(searchString) // by first name
                                     || u.LastName.Contains(searchString) // by last name
                                     || (u.FirstName + u.LastName).Contains(searchString) // by first and last name combined
                                  select u)
                    .Distinct()
                    .OrderByDescending(u => u.Email);

                // Return the Index view with the list of search results.
                ViewBag.currentFilter = "Search Results";
                return View(userSearch);
            }
            else if (currentFilter == "Students")
            {
                var userSearch = (from u in _context.User
                                  where
                                    u.IsAdmin == false
                                  select u)
                    .Distinct()
                    .OrderByDescending(u => u.Email);
                return View(userSearch);
            }
            else if (currentFilter == "Administrators")
            {
                var userSearch = (from u in _context.User
                                  where
                                    u.IsAdmin == true
                                  select u)
                    .Distinct()
                    .OrderByDescending(u => u.Email);
                return View(userSearch);
            }


            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            AddUserIfNotExists(User.Identity.Name);

            var user = await _context.User
                .Include(u => u.PrizesRedeemed)
                .Include(u => u.EventsAttended)
                .FirstOrDefaultAsync(m => m.UserID == id);

            var vm = new UserProfileViewModel();
            vm.User = user;
            vm.Events = new List<Event>();
            vm.Prizes = new List<Prize>();

            foreach (EventAttended ev in user.EventsAttended)
            {
                var e = _context.Event.Where(eve => eve.ID == ev.EventID).First();
                vm.Events.Add(e);
            }

            foreach (PrizeRedeemed prize in user.PrizesRedeemed)
            {
                var p = _context.Prize.Where(pr => pr.ID == prize.PrizeID).First();
                vm.Prizes.Add(p);
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Users/Me
        public async Task<IActionResult> Me()
        {
            AddUserIfNotExists(User.Identity.Name);

            var user = await _context.User
                .Include(u => u.PrizesRedeemed)
                .Include(u => u.EventsAttended)
                .FirstOrDefaultAsync(m => m.Email == User.Identity.Name);

            var vm = new UserProfileViewModel();
            vm.User = user;
            vm.Events = new List<Event>();
            vm.Prizes = new List<Prize>();

            foreach (EventAttended ev in user.EventsAttended)
            {
                var e = _context.Event.Where(eve => eve.ID == ev.EventID).First();
                vm.Events.Add(e);
            }

            foreach (PrizeRedeemed prize in user.PrizesRedeemed)
            {
                var p = _context.Prize.Where(pr => pr.ID == prize.PrizeID).First();
                vm.Prizes.Add(p);
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [NonAction]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,Email,FirstName,LastName,IsAdmin,Points,EventsAttendedIDs,PrizesRedeemedIDs,EventsCreatedIDs")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            var usr = _context.User.Where(u => u.UserID == id).First();

            if (ModelState.IsValid)
            {
                try
                {
                    usr.UserID = user.UserID;
                    usr.FirstName = user.FirstName;
                    usr.LastName = user.LastName;
                    usr.Points = user.Points;
                    _context.Update(usr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
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
            return View(user);
        }

        public async Task<IActionResult> ToggleUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            user.IsAdmin = !user.IsAdmin;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = user.UserID });
        }

        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Policy = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserID == id);
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
