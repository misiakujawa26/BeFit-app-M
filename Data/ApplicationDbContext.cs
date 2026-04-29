using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using befit.Models;

namespace befit.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<befit.Models.ExerciseType> ExerciseType { get; set; } = default!;
        public DbSet<befit.Models.Session> Session { get; set; } = default!;
        public DbSet<befit.Models.Exercise> Exercise { get; set; } = default!;
    }
}
