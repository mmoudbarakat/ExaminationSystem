using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ExaminationSystem.Data
{
    public class Context : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Question> Questions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=mahmoud; Initial Catalog = ExaminationSystem; Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .LogTo(Log => Debug.WriteLine(Log), LogLevel.Information)
                .EnableSensitiveDataLogging();
        }
    }
}
