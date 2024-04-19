namespace GestionEmpledo.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public int IdRol { get; set; }
        
        // Propiedad de navegación al rol del empleado
        public Rol Rol { get; set; }
    }
}
