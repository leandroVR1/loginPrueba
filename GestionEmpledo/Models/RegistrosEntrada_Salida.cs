using System;

namespace GestionEmpledo.Models
{
    public class RegistrosEntrada_Salida
    {
        public int Id { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public int IdEmpleado { get; set; } // Id del empleado asociado

        public virtual Empleado Empleado { get; set; } // Referencia de navegación al empleado
    }
}
