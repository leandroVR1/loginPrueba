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
        public DbSet<Rol> Roles { get; set; } // Agrega DbSet para la entidad Rol
        public DbSet<RegistrosEntrada_Salida> RegistrosEntrada_Salida { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar el nombre de la tabla y las relaciones si es necesario
            modelBuilder.Entity<RegistrosEntrada_Salida>()
                .ToTable("RegistrosEntrada_Salida")
                .HasKey(e => e.Id); // Ajusta el nombre de la tabla según sea necesario

            // Configurar la relación entre RegistrosEntrada_Salida y Empleado
            modelBuilder.Entity<RegistrosEntrada_Salida>()
                .HasOne(r => r.Empleado)  // Una entrada y salida pertenece a un empleado
                .WithMany()  // Un empleado puede tener múltiples entradas y salidas
                .HasForeignKey(r => r.IdEmpleado); // Clave externa en RegistrosEntrada_Salida que apunta a la tabla Empleado

            // Configurar la relación entre Empleado y Rol
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Rol)  // Un empleado pertenece a un rol
                .WithMany()  // Un rol puede tener múltiples empleados
                .HasForeignKey(e => e.IdRol); // Clave externa en Empleado que apunta a la tabla Rol
        }
    }
}
