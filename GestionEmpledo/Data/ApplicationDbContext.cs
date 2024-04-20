using Microsoft.EntityFrameworkCore;
using GestionEmpledo.Models;

namespace GestionEmpledo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<RegistrosEntrada_Salida> RegistrosEntrada_Salida { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RegistrosEntrada_Salida>()
                .ToTable("RegistrosEntrada_Salida")
                .HasKey(e => e.Id);

            modelBuilder.Entity<RegistrosEntrada_Salida>()
                .HasOne(r => r.Empleado)
                .WithMany()
                .HasForeignKey(r => r.IdEmpleado);

            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Rol)
                .WithMany(r => r.Empleados) // Aquí se define la relación inversa
                .HasForeignKey(e => e.IdRol);

            modelBuilder.Entity<Rol>()
                .ToTable("Rol")
                .HasKey(r => r.IdRol);
        }
    }
}
