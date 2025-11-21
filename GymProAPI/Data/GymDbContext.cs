using GymProAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GymProAPI.Data
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        public DbSet<Socio> Socios { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Socios
            modelBuilder.Entity<Socio>()
                .ToTable("Socios")
                .HasKey(s => s.SocioID);

            // Pagos
            modelBuilder.Entity<Pago>()
                .ToTable("Pagos")
                .HasKey(p => p.PagoID);

            // Relación: un socio tiene muchos pagos
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Socio)
                .WithMany(s => s.Pagos) // 👈 ahora sí existe
                .HasForeignKey(p => p.SocioID)
                .OnDelete(DeleteBehavior.Cascade);

            // Restricciones
            modelBuilder.Entity<Pago>()
                .Property(p => p.MetodoPago)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasColumnType("decimal(10,2)");
        }
    }
}