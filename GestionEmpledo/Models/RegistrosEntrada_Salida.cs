using System.ComponentModel.DataAnnotations;

namespace GestionEmpledo.Models
{
    public class RegistrosEntrada_Salida
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria.")]
        public DateTime FechaEntrada { get; set; }

        public DateTime? FechaSalida { get; set; } // Puede ser nulo si el empleado aún no ha salido

        [Required(ErrorMessage = "El ID del empleado es obligatorio.")]
        public int IdEmpleado { get; set; } // Esta sería la clave foránea del empleado

        // Propiedad de navegación al empleado asociado
        public Empleado Empleado { get; set; }
    }
}
