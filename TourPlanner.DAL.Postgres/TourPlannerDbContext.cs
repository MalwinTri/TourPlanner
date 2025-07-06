using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.DAL.Postgres
{
    public class TourPlannerDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        public TourPlannerDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>().ToTable("tours").HasKey(t => t.Id);
            modelBuilder.Entity<TourLog>().ToTable("tourlogs").HasKey(tl => tl.Id);

        }
    }
}