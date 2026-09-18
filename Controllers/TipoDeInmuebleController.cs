using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria_grupo_9.Controllers
{
    public class TipoDeInmuebleController : Controller
    {
        private readonly IRepositorioTipoDeInmueble _repositorio;

        public TipoDeInmuebleController(IRepositorioTipoDeInmueble repositorio)
        {
            _repositorio = repositorio;
        }

        public IActionResult Index(string busqueda, string habilitadoFiltro, int pagina = 1)
        {
            try
            {
                const int tamPagina = 5;

                bool? habilitado = habilitadoFiltro switch
                {
                    "habilitados" => true,
                    "deshabilitados" => false,
                    _ => null
                };

                bool hayFiltros = !string.IsNullOrWhiteSpace(busqueda) || habilitado.HasValue;
                IList<TipoDeInmueble> tipos;

                if (hayFiltros)
                {
                    tipos = _repositorio.Buscar(busqueda, habilitado);
                    ViewBag.TotalPaginas = 1;
                }
                else
                {
                    int totalRegistros = _repositorio.ObtenerCantidad();
                    int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);
                    tipos = _repositorio.ObtenerLista(pagina, tamPagina);
                    ViewBag.TotalPaginas = totalPaginas;
                }

                ViewBag.PaginaActual = pagina;
                ViewBag.Busqueda = busqueda;
                ViewBag.HabilitadoFiltro = habilitadoFiltro;

                return View(tipos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipos de inmueble: {ex.Message}");
                return View(new List<TipoDeInmueble>());
            }
        }

        public IActionResult Details(int id)
        {
            var tipo = _repositorio.ObtenerPorId(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoDeInmueble tipo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorio.Alta(tipo);
                    return RedirectToAction(nameof(Index));
                }
                return View(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear tipo de inmueble: {ex.Message}");
                return View(tipo);
            }
        }

        public IActionResult Edit(int id)
        {
            var tipo = _repositorio.ObtenerPorId(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoDeInmueble tipo)
        {
            try
            {
                tipo.IdTipoInmueble = id;
                if (ModelState.IsValid)
                {
                    _repositorio.Modificacion(tipo);
                    return RedirectToAction(nameof(Index));
                }
                return View(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar tipo de inmueble: {ex.Message}");
                return View(tipo);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var tipo = _repositorio.ObtenerPorId(id);
            if (tipo == null) return NotFound();

            ViewBag.EnUso = _repositorio.ContarInmueblesQueLoUsan(id);
            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                int enUso = _repositorio.ContarInmueblesQueLoUsan(id);
                if (enUso > 0)
                {
                    TempData["Error"] = $"No se puede eliminar: hay {enUso} inmueble(s) usando este tipo.";
                    return RedirectToAction(nameof(Index));
                }

                _repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar tipo de inmueble: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}