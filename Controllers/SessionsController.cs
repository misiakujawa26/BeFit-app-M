using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using befit.Data;
using befit.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace befit.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _context.Session
                .Include (s => s.User)
                .Where(s =>s.UserId == userId)
                .ToListAsync());
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }
            //Zabezpieczenie dostępu
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (session.UserId != userId)
            {
                return Forbid();
            }

            return View(session);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Start,End")] Session session)
        {
            session.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            //Zabezpieczenie dostepu
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (session.UserId != userId)
            {
                return Forbid();
            }
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Start,End")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }
            //Pobranie z bazy 
            var sessionFromDb = await _context.Session.FindAsync(id);

            if (sessionFromDb == null)
            {
                return NotFound();
            }
            //Sprawdzenie właściciela
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sessionFromDb.UserId != userId)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //Aktualizacja dozwolonych pól
                    sessionFromDb.Start = session.Start;
                    sessionFromDb.End = session.End;
                    _context.Update(sessionFromDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(sessionFromDb.Id))
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
            return View(session);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }
            //Zabezpieczenie dostepu
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (session.UserId != userId)
            {
                return Forbid();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Session.FindAsync(id);
            if (session == null)//zmiana
            {
               // _context.Session.Remove(session);
               return NotFound();
            }
            //Zabezpieczenie przed usuwaniem danych
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (session.UserId != userId)
            {
                return Forbid();
            }

            _context.Session.Remove(session);


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Session.Any(e => e.Id == id);
        }
    }
}
