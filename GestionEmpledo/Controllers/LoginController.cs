using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GestionEmpledo.Models;
using GestionEmpledo.Data; // Agrega este using para acceder a HttpContext.Session
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using GestionEmpledo.ViewModels; // Para EmpleadoCreateViewModel
using Microsoft.AspNetCore.Mvc.Rendering;






namespace GestionEmpledo.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {   
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Login
         [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Login
        
        [HttpPost]
        [AllowAnonymous]


public async Task<IActionResult> Login(string correo, string contraseña)
{
    if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
    {
        TempData["Error"] = "Por favor, ingrese el correo y la contraseña.";
        return View("Index");
    }

    var empleado = await _context.Empleados.FirstOrDefaultAsync(u => u.Correo == correo && u.Contraseña == contraseña);

    if (empleado != null)
    {
        HttpContext.Session.SetInt32("EmpleadoId", empleado.Id);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, empleado.Id.ToString()),
            new Claim(ClaimTypes.Name, empleado.Nombre)
        };

        if (empleado.IdRol == 1)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Administrador"));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Historial");
        }
        else if (empleado.IdRol == 2)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Empleado"));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("EntradaSalida");
        }
    }

    TempData["Error"] = "Correo o contraseña incorrectos";
    return View("Index");
}

[Authorize(Roles = "Empleado")]
public IActionResult EntradaSalida()
{
    // Verificar si el usuario tiene el rol adecuado antes de permitir el acceso
    if (!User.IsInRole("Empleado"))
    {
        return RedirectToAction("AccessDenied", "Error");
    }

    var model = new RegistrosEntrada_Salida();
    return View(model);
}


        // POST: /Login/GuardarEntradaSalida
         [AllowAnonymous]
        public IActionResult GuardarEntradaSalida(RegistrosEntrada_Salida model)
        {
            // Obtener el Id del empleado de la sesión
            var empleadoId = HttpContext.Session.GetInt32("EmpleadoId");
            Console.WriteLine($"Empleado ID recuperado de la sesión: {empleadoId}");

            if (!empleadoId.HasValue)
            {
                // No se pudo obtener el Id del empleado de la sesión
                ModelState.AddModelError("", "No se pudo obtener el Id del empleado.");
                return RedirectToAction("EntradaSalida", model);
            }

            // Asignar el Id del empleado al campo IdEmpleado del modelo
            model.IdEmpleado = empleadoId.Value;

            // Guardar en la base de datos
            _context.RegistrosEntrada_Salida.Add(model);
            _context.SaveChanges();

            // Redireccionar a la página de Historial
            return RedirectToAction("Historial");
        }

        // GET: /Login/Historial
           [Authorize(Roles = "Administrador")]
             public IActionResult Historial()
        {
            // Obtener todos los registros de entrada y salida de la base de datos
            var registros = _context.RegistrosEntrada_Salida
                .Include(r => r.Empleado) // Incluir información del empleado asociado
                .ToList();

            return View(registros);
        }

        // GET: /Login/Empleados
           [Authorize(Roles = "Administrador")]
        public IActionResult Empleados()
        {
            // Obtener todos los empleados de la base de datos
            var empleados = _context.Empleados.ToList();

            return View(empleados);
        }

        // POST: /Login/Create
       
[Authorize(Roles = "Administrador")]
public IActionResult Create()
{
    var viewModel = new EmpleadoCreateViewModel
    {
        Roles = _context.Roles.Select(r => new SelectListItem
        {
            Value = r.IdRol.ToString(),
            Text = r.Descripcion
        }).ToList()
    };
    return View(viewModel);
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Administrador")]
public async Task<IActionResult> Create(EmpleadoCreateViewModel viewModel)
{
   
    
        
            _context.Empleados.Add(viewModel.Empleado);
            await _context.SaveChangesAsync();
            return RedirectToAction("Empleados");
        
            ModelState.AddModelError("", "Error.");
        

  
    viewModel.Roles = _context.Roles.Select(r => new SelectListItem
    {
        Value = r.IdRol.ToString(),
        Text = r.Descripcion
    }).ToList();

    return View(viewModel);
}







        // POST: /Login/CerrarSesion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            // Remueve el ID de empleado de la sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("EmpleadoId");
            return RedirectToAction("Index", "Home");
        }
    }
}
