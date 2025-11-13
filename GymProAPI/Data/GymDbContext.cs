using GymProAPI.Models;
using Microsoft.EntityFrameworkCore;
using GymProAPI.Models;

namespace GymProAPI.Data
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        public DbSet<Socio> Socios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Socio>().ToTable("Socios");
        }
    }
}