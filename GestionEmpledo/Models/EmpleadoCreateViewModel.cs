using GestionEmpledo.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // Importante para SelectListItem
using System.Collections.Generic;

namespace GestionEmpledo.ViewModels // Asegúrate de que el namespace sea correcto
{
   public class EmpleadoCreateViewModel
{
    public Empleado Empleado { get; set; }
    public List<SelectListItem> Roles { get; set; }
}

}
