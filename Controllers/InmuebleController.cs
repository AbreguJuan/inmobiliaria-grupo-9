using System;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;

namespace inmobiliaria_grupo_9.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble _repositorioInmueble;
        private readonly IRepositorioPropietario _repositorioPropietario;

        public InmuebleController(IRepositorioInmueble repositorioInmueble, IRepositorioPropietario repositorioPropietario)
        {
            _repositorioInmueble = repositorioInmueble;
            _repositorioPropietario = repositorioPropietario;
        }

        public ActionResult Index(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = _repositorioInmueble.ObtenerLista(paginaNro, tamPagina);
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error")) ViewBag.Error = TempData["Error"];
            return View(lista);
        }

        public ActionResult Details(int id)
        {
            var entidad = id == 0 ? new Inmueble() : _repositorioInmueble.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            return View(entidad);
        }

        public ActionResult Create()
        {
            try
            {
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
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
        public ActionResult Create(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioInmueble.Alta(entidad);
                    TempData["Mensaje"] = "Inmueble creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                ViewBag.Error = ex.Message;
                return View(entidad);
            }
        }

        public ActionResult Edit(int id)
        {
            var entidad = _repositorioInmueble.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            
            ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
            return View(entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble entidad)
        {
            try
            {
                entidad.IdInmueble = id;
                if (ModelState.IsValid)
                {
                    _repositorioInmueble.Modificacion(entidad);
                    TempData["Mensaje"] = "Inmueble modificado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = _repositorioPropietario.ObtenerLista(1, 100);
                ViewBag.Error = ex.Message;
                return View(entidad);
            }
        }

        public ActionResult Delete(int id)
        {
            var entidad = _repositorioInmueble.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            return View(entidad);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
    }
}