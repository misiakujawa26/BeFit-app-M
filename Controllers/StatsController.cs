using Microsoft.AspNetCore.Mvc;
using befit.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using befit.Models;

namespace befit.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DateTime fromDate = DateTime.Now.AddDays(-28);

            var data = await _context.Exercise
                .Include(e => e.Session)
                .Include(e => e.ExerciseType)
                .Where(e =>
                    e.Session.UserId == userId &&
                    e.Session.Start >= fromDate)
                .ToListAsync();

            var stats = data
     .GroupBy(e => e.ExerciseType.Name)
     .Select(g => new ExerciseStatsViewModel   
     {
         Name = g.Key,
         Count = g.Count(),
         TotalReps = g.Sum(x => x.NumOfSeries * x.NumOfReps),
         AvgWeight = g.Average(x => x.Weight),
         MaxWeight = g.Max(x => x.Weight)
     })
                 .ToList();

            return View(stats);
        }
    }
}