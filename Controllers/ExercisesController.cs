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
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exercises
        public async Task<IActionResult> Index()
        {
            //Pobranie uzytkownika
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var applicationDbContext = _context.Exercise.Include(e => e.ExerciseType).Include(e => e.Session).Where(e => e.Session.UserId == userId);//Filtrowanie po uzytkowniku
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }
            //Zabezpieczenie dostepu
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exercise.Session.UserId != userId)
            {
                return Forbid();
            }


            return View(exercise);
        }

        // GET: Exercises/Create
        public IActionResult Create()
        {
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Name");
            ViewData["SessionId"] = new SelectList(_context.Session, "Id", "Start");
            return View();
        }

        // POST: Exercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Weight,NumOfSeries,NumOfReps,ExerciseTypeId,SessionId")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                //Sprawdzenie sesji
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var session = await _context.Session.FirstOrDefaultAsync(s => s.Id == exercise.SessionId);
                if (session == null || session.UserId != userId)
                {
                    return Forbid(); // DODANE - blokada cudzej sesji
                }
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Id", exercise.ExerciseTypeId);
            ViewData["SessionId"] = new SelectList(_context.Session, "Id", "Id", exercise.SessionId);
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .Include(e => e.Session) 
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }
            //Sprawdzenie uzytkownika
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exercise.Session.UserId != userId)
            {
                return Forbid();
            }

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Id", exercise.ExerciseTypeId);
            ViewData["SessionId"] = new SelectList(_context.Session, "Id", "Id", exercise.SessionId);
            return View(exercise);
        }

        // POST: Exercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Weight,NumOfSeries,NumOfReps,ExerciseTypeId,SessionId")] Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }
            //Pobranie z bazy
            var exerciseFromDb = await _context.Exercise
              .Include(e => e.Session)
              .FirstOrDefaultAsync(e => e.Id == id);
            if (exerciseFromDb == null)
            {
                return NotFound();
            }
            //Sprawdzenie wlasciciela
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exerciseFromDb.Session.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Aktualizacja pól
                    exerciseFromDb.Weight = exercise.Weight;
                    exerciseFromDb.NumOfSeries = exercise.NumOfSeries;
                    exerciseFromDb.NumOfReps = exercise.NumOfReps;
                    exerciseFromDb.ExerciseTypeId = exercise.ExerciseTypeId;
                    exerciseFromDb.SessionId = exercise.SessionId;
                    _context.Update(exerciseFromDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exerciseFromDb.Id))
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
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Id", exercise.ExerciseTypeId);
            ViewData["SessionId"] = new SelectList(_context.Session, "Id", "Id", exercise.SessionId);
            return View(exercise);
        }

        // GET: Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }
            //Zabezpieczenie dostepu
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exercise.Session.UserId != userId)
            {
                return Forbid();
            }


            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.Exercise.Include(e => e.Session).FirstOrDefaultAsync(e => e.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }
            //Zabezpieczenie 
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exercise.Session.UserId != userId)
            {
                return Forbid();
            }
            _context.Exercise.Remove(exercise);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercise.Any(e => e.Id == id);
        }
    }
}
