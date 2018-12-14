using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlashPoints.Data;
using FlashPoints.Models;

using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlashPoints.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            AddUserIfNotExists(User.Identity.Name);
            var events = await _context.Event.Where(e => e.Approved == true)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
            return View(events);
        }

        // GET: Events
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> StudentEvents()
        {
            AddUserIfNotExists(User.Identity.Name);
            var events = await _context.Event.Where(e => e.Approved != true)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
            return View(events);
        }

        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> ApproveEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.Approved = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("StudentEvents");
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            

            var user = _context.User.Where(u => u.Email == User.Identity.Name);
            if (user.First().IsAdmin == true)
            {
                ViewBag.isAdmin = true;
            }

            return View(@event);
        }

        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> QRCode(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            var user = _context.User.Where(u => u.Email == User.Identity.Name).First();

            if (@event.Creator == User.Identity.Name || user.IsAdmin)
            {
                string qrcode = @event.Title;

                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }

                return View(@event);
            } else
            {
                return View("AccessDenied");
            }
        }

        [Authorize(Policy = "Administrator")]
        public IActionResult AdminEvent()
        {
            return View();
        }

        [Authorize(Policy = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> AdminEvent(Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.Approved = true;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = @event.ID });
            }
            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,StartDateTime,EndDateTime,PointValue,QRcode,Approved,Creator,Location,NumberAttended")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();

                string subject = "A new FlashPoints Event Request is waiting to be approved!";
                string body = $"<a href='https://flashpoints-web-app.azurewebsites.net/Events/Details/{@event.ID}'>Click here to view the request.</ a ><br /> <br /> <br />";
                EmailSender.SendMail("flashpointscoordinators@gmail.com", subject, body);


                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,StartDateTime,EndDateTime,PointValue,QRcode,Approved,Creator,Location,NumberAttended")] Event @event)
        {
            if (id != @event.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.ID))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.ID == id);
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
