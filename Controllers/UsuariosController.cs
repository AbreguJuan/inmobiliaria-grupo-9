using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using inmobiliaria_grupo_9.Models;
using Microsoft.Extensions.Logging;

namespace inmobiliaria_grupo_9.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IRepositorioUsuario _repositorio;

        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioUsuario repositorio, ILogger<UsuariosController> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _repositorio = repositorio;
            _logger = logger;
        }

        // GET: Usuarios
        [Authorize(Policy = "Administrador")]
        public ActionResult Index(int pagina = 1)
        {
            var usuarios = _repositorio.ObtenerLista(pagina);
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var e = _repositorio.ObtenerPorId(id);
            if (e == null) return NotFound();
            return View(e);
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(u);
            }
                
            try
            {
                // Encriptar la clave
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: u.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"] ?? "S@ltDefault123!"),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                
                u.Clave = hashed;
                
                int res = _repositorio.Alta(u);
                
                // Guardar el Avatar si se subió uno
                if (u.AvatarFile != null && u.IdUsuario > 0)
                {
                    string wwwPath = _environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Avatares");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    
                    string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads/Avatares", fileName).Replace("\\", "/");
                    
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    _repositorio.Modificacion(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                ViewBag.Error = ex.Message;
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(u);
            }
        }

        // GET: Usuarios/Perfil
        [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var u = _repositorio.ObtenerPorId(userId);
            if (u == null) return NotFound();
            
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(nameof(Edit), u);
        }

        // GET: Usuarios/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = _repositorio.ObtenerPorId(id);
            if (u == null) return NotFound();
            
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = nameof(Edit);
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                if (!User.IsInRole("Administrador"))
                {
                    vista = nameof(Perfil);
                    if (currentUserId != id) 
                        return RedirectToAction(nameof(Index), "Home");
                }
                
                u.IdUsuario = id;

                var usuarioOriginal = _repositorio.ObtenerPorId(id);
                if (usuarioOriginal == null) return NotFound();

                if (string.IsNullOrEmpty(u.Clave))
                {
                    u.Clave = usuarioOriginal.Clave;
                }
                else
                {
                    u.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"] ?? "S@ltDefault123!"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                }

                if (u.AvatarFile != null)
                {
                    string wwwPath = _environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Avatares");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads/Avatares", fileName).Replace("\\", "/");

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                }
                else
                {
                    u.Avatar = usuarioOriginal.Avatar;
                }

                _repositorio.Modificacion(u);

                if (currentUserId == id)
                {
                    return RedirectToAction("Logout");
                }

                return RedirectToAction(vista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el usuario");
                ViewBag.Error = ex.Message;
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(vista, u);
            }
        }

        // GET: Usuarios/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var u = _repositorio.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var usuario = _repositorio.ObtenerPorId(id);
                if (usuario != null && !string.IsNullOrEmpty(usuario.Avatar))
                {
                    var ruta = Path.Combine(_environment.WebRootPath, usuario.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(ruta))
                        System.IO.File.Delete(ruta);
                }
                
                _repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            try
            {
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : (TempData["returnUrl"] ?? "").ToString();
                
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"] ?? "S@ltDefault123!"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var e = _repositorio.ObtenerPorEmail(login.Usuario);
                    if (e == null || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        TempData["returnUrl"] = returnUrl;
                        return View(login);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, e.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                        new Claim("AvatarUrl", e.Avatar ?? "")
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.NameIdentifier, ClaimTypes.Role);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                        
                    TempData.Remove("returnUrl");
                    return Redirect(returnUrl ?? "/");
                }
                
                TempData["returnUrl"] = returnUrl;
                return View(login);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(login);
            }
        }

        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}