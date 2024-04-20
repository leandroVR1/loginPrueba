namespace GestionEmpledo.Models
{
    public class Rol
{
    public int IdRol { get; set; }
    public string Descripcion { get; set; }

    // Lista de empleados
    public List<Empleado> Empleados { get; set; }
}
}
