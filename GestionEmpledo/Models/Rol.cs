namespace GestionEmpledo.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        
        // Propiedad de navegación a los empleados con este rol
        public ICollection<Empleado> Empleados { get; set; }
    }
}
