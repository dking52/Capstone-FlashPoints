using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlashPoints.Models;
using FlashPoints.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlashPoints.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            AddUserIfNotExists(User.Identity.Name);
            var user = _context.User.Where(u => u.Email == User.Identity.Name);
            if (user.First().IsAdmin == true)
            {
                ViewBag.isAdmin = "true";
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
