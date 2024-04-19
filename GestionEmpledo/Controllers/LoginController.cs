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
        ModelState.AddModelError("", "Por favor, ingrese el correo y la contraseña.");
        return View("Index");
    }

    var empleado = _context.Empleados.FirstOrDefault(u => u.Correo == correo && u.Contraseña == contraseña);

    if (empleado != null)
    {
        // Autenticación exitosa
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, empleado.Id.ToString()),
            new Claim(ClaimTypes.Name, empleado.Nombre)
        };

        // Agregar el rol como claim si existe
        if (empleado.Rol != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, empleado.Rol.ToString()));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // Redirige al usuario según su rol
        if (empleado.Rol == Rol.Administrador)
        {
            return RedirectToAction("Index", "Home"); // Por ejemplo, redirige al Home para el admin
        }
        else if (empleado.Rol == Rol.Empleado)
        {
            return RedirectToAction("EntradaSalida");
        }
    }

    // Credenciales inválidas
    ModelState.AddModelError("", "Correo o contraseña incorrectos");
    return View("Index");
}

        // GET: /Login/EntradaSalida
       // Ejemplo de aplicación de política de autorización para la acción EntradaSalida
[Authorize(Roles = "Empleado")]
public IActionResult EntradaSalida()
{
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
             public IActionResult Historial()
        {
            // Obtener todos los registros de entrada y salida de la base de datos
            var registros = _context.RegistrosEntrada_Salida
                .Include(r => r.Empleado) // Incluir información del empleado asociado
                .ToList();

            return View(registros);
        }

        // GET: /Login/Empleados
        public IActionResult Empleados()
        {
            // Obtener todos los empleados de la base de datos
            var empleados = _context.Empleados.ToList();

            return View(empleados);
        }

        // POST: /Login/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _context.Empleados.Add(empleado);
                _context.SaveChanges();
                return RedirectToAction("Empleados");
            }
            return View(empleado);
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
