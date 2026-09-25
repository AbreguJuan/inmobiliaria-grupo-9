using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_grupo_9.Models;

namespace inmobiliaria_grupo_9.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble _repositorioInmueble;
        private readonly IRepositorioPropietario _repositorioPropietario;
        private readonly IRepositorioTipoDeInmueble _repositorioTipoDeInmueble;
        private readonly IRepositorioImagenInmueble _repositorioImagen;
        private readonly IRepositorioReserva _repositorioReserva;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InmuebleController(
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPropietario repositorioPropietario,
            IRepositorioTipoDeInmueble repositorioTipoDeInmueble,
            IRepositorioImagenInmueble repositorioImagen,
            IRepositorioReserva repositorioReserva,
            IWebHostEnvironment webHostEnvironment)
        {
            _repositorioInmueble = repositorioInmueble;
            _repositorioPropietario = repositorioPropietario;
            _repositorioTipoDeInmueble = repositorioTipoDeInmueble;
            _repositorioImagen = repositorioImagen;
            _repositorioReserva = repositorioReserva;
            _webHostEnvironment = webHostEnvironment;
        }

        private string GuardarArchivo(IFormFile archivo)
        {
            string carpeta = Path.Combine(_webHostEnvironment.WebRootPath, "images", "inmuebles");
            Directory.CreateDirectory(carpeta);

            string nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
            string rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            return $"/images/inmuebles/{nombreArchivo}";
        }

        public ActionResult Index(
            string busqueda, decimal? precio, string operadorPrecio, string habilitadoFiltro,
            int? ambientesMinimo, decimal? metrosMinimo, decimal? metrosMaximo,
            DateTime? fechaDesde, DateTime? fechaHasta,
            int? cupoMinimo, decimal? latitud, decimal? longitud, decimal? radioKm,
            int? idPropietario,
            int paginaNro = 1, int tamPagina = 10)
        {
            bool? habilitado = habilitadoFiltro switch
            {
                "habilitados" => true,
                "deshabilitados" => false,
                _ => null
            };

            bool hayFiltros = !string.IsNullOrWhiteSpace(busqueda) || precio.HasValue || habilitado.HasValue
                || ambientesMinimo.HasValue || metrosMinimo.HasValue || metrosMaximo.HasValue
                || (fechaDesde.HasValue && fechaHasta.HasValue)
                || cupoMinimo.HasValue || (latitud.HasValue && longitud.HasValue && radioKm.HasValue)
                || idPropietario.HasValue;

            IList<Inmueble> lista;

            if (hayFiltros)
            {
                lista = _repositorioInmueble.Buscar(
                    busqueda, precio, operadorPrecio, habilitado,
                    ambientesMinimo, metrosMinimo, metrosMaximo,
                    fechaDesde, fechaHasta,
                    cupoMinimo, latitud, longitud, radioKm,
                    idPropietario);

                // Con filtros no usamos paginación de a servidor, se muestra todo el resultado
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
            }
            else
            {
                lista = _repositorioInmueble.ObtenerLista(paginaNro, tamPagina);

                int totalRegistros = _repositorioInmueble.ObtenerCantidad();
                ViewBag.PaginaActual = paginaNro;
                ViewBag.TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamPagina);
            }

            ViewBag.Busqueda = busqueda;
            ViewBag.Precio = precio;
            ViewBag.OperadorPrecio = operadorPrecio;
            ViewBag.HabilitadoFiltro = habilitadoFiltro;
            ViewBag.AmbientesMinimo = ambientesMinimo;
            ViewBag.MetrosMinimo = metrosMinimo;
            ViewBag.MetrosMaximo = metrosMaximo;
            ViewBag.FechaDesde = fechaDesde;
            ViewBag.FechaHasta = fechaHasta;
            ViewBag.CupoMinimo = cupoMinimo;
            ViewBag.Latitud = latitud;
            ViewBag.Longitud = longitud;
            ViewBag.RadioKm = radioKm;
            ViewBag.IdPropietario = idPropietario;

            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error")) ViewBag.Error = TempData["Error"];

            ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);

            return View(lista);
        }

        public ActionResult Details(int id)
        {
            var entidad = id == 0 ? new Inmueble() : _repositorioInmueble.ObtenerPorId(id);
            if (entidad == null) return NotFound();

            if (id != 0)
            {
                entidad.Imagenes = _repositorioImagen.ObtenerPorInmueble(id);

                var reservas = _repositorioReserva.ObtenerPorInmueble(id)
                    .Where(r => !r.Finalizada)
                    .Select(r => new { desde = r.Desde.ToString("yyyy-MM-dd"), hasta = r.Hasta.ToString("yyyy-MM-dd") });

                ViewBag.RangosOcupados = System.Text.Json.JsonSerializer.Serialize(reservas);
            }

            return View(entidad);
        }

        public ActionResult Create()
        {
            try
            {
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                ViewBag.Tipos = _repositorioTipoDeInmueble.ObtenerLista(1, 100).Where(t => t.Habilitado).ToList();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble entidad, IFormFile? fotoPortada, List<IFormFile>? fotosGaleria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (fotoPortada != null)
                    {
                        entidad.FotoPortada = GuardarArchivo(fotoPortada);
                    }

                    _repositorioInmueble.Alta(entidad);

                    if (fotosGaleria != null)
                    {
                        foreach (var foto in fotosGaleria)
                        {
                            if (foto.Length > 0)
                            {
                                string url = GuardarArchivo(foto);
                                _repositorioImagen.Alta(new ImagenInmueble { IdInmueble = entidad.IdInmueble, Url = url });
                            }
                        }
                    }

                    TempData["Mensaje"] = "Inmueble creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                ViewBag.Tipos = _repositorioTipoDeInmueble.ObtenerLista(1, 1000).Where(t => t.Habilitado).ToList();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                ViewBag.Tipos = _repositorioTipoDeInmueble.ObtenerLista(1, 1000).Where(t => t.Habilitado).ToList();
                ViewBag.Error = ex.Message;
                return View(entidad);
            }
        }

        public ActionResult Edit(int id)
        {
            var entidad = _repositorioInmueble.ObtenerPorId(id);
            if (entidad == null) return NotFound();

            ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
            ViewBag.Tipos = _repositorioTipoDeInmueble.ObtenerLista(1, 100).Where(t => t.Habilitado).ToList();
            return View(entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble entidad, IFormFile? fotoPortada, List<IFormFile>? fotosGaleria)
        {
            try
            {
                entidad.IdInmueble = id;
                if (ModelState.IsValid)
                {
                    if (fotoPortada != null)
                    {
                        entidad.FotoPortada = GuardarArchivo(fotoPortada);
                    }
                    else
                    {
                        var existente = _repositorioInmueble.ObtenerPorId(id);
                        entidad.FotoPortada = existente?.FotoPortada;
                    }

                    _repositorioInmueble.Modificacion(entidad);

                    if (fotosGaleria != null)
                    {
                        foreach (var foto in fotosGaleria)
                        {
                            if (foto.Length > 0)
                            {
                                string url = GuardarArchivo(foto);
                                _repositorioImagen.Alta(new ImagenInmueble { IdInmueble = id, Url = url });
                            }
                        }
                    }

                    TempData["Mensaje"] = "Inmueble modificado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                ViewBag.Tipos = _repositorioTipoDeInmueble.ObtenerLista(1, 1000).Where(t => t.Habilitado).ToList();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                ViewBag.Tipos = _repositorioTipoDeInmueble.ObtenerLista(1, 1000).Where(t => t.Habilitado).ToList();
                ViewBag.Error = ex.Message;
                return View(entidad);
            }
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            var entidad = _repositorioInmueble.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            return View(entidad);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorioInmueble.Baja(id);
                TempData["Mensaje"] = "Inmueble eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar el inmueble. " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult PorPropietario(int id)
        {
            var lista = _repositorioInmueble.BuscarPorPropietario(id);
            return View("Index", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarImagen(int idImagen, int idInmueble)
        {
            _repositorioImagen.Baja(idImagen);
            return RedirectToAction(nameof(Details), new { id = idInmueble });
        }

        public IActionResult SinReservas(int dias = 30)
        {
            var lista = _repositorioInmueble.ObtenerSinReservas(dias);
            ViewBag.Dias = dias;
            return View(lista);
        }

        [HttpGet]
        public IActionResult BuscarJson(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new object[0]);

            var resultados = _repositorioInmueble.Buscar(term)
                .Take(15)
                .Select(i => new { id = i.IdInmueble, texto = i.ToString() });

            return Json(resultados);
        }
    }
}