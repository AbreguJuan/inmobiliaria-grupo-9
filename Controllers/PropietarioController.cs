using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria_grupo_9.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario _repositorioPropietario;

        public PropietarioController(IRepositorioPropietario repositorioPropietario)
        {
            _repositorioPropietario = repositorioPropietario;
        }

        public IActionResult Index(string busqueda, int pagina = 1)
        {
            try
            {
                const int tamPagina = 5;
                IList<Propietario> propietarios;

                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    propietarios = _repositorioPropietario.Buscar(busqueda);
                    ViewBag.TotalPaginas = 1;
                }
                else
                {
                    int totalRegistros = _repositorioPropietario.ObtenerCantidad();
                    int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);
                    propietarios = _repositorioPropietario.ObtenerLista(pagina, tamPagina);
                    ViewBag.TotalPaginas = totalPaginas;
                }

                ViewBag.PaginaActual = pagina;
                ViewBag.Busqueda = busqueda;

                return View(propietarios);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener propietarios: {ex.Message}");
                return View(new List<Propietario>());
            }
        }

        public IActionResult Details(int id)
        {
            var propietario = _repositorioPropietario.ObtenerPorId(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioPropietario.Alta(propietario);
                    return RedirectToAction(nameof(Index));
                }
                return View(propietario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear propietario: {ex.Message}");
                return View(propietario);
            }
        }

        public IActionResult Edit(int id)
        {
            var propietario = _repositorioPropietario.ObtenerPorId(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioPropietario.Modificacion(propietario);
                    return RedirectToAction(nameof(Index));
                }
                return View(propietario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar propietario: {ex.Message}");
                return View(propietario);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var propietario = _repositorioPropietario.ObtenerPorId(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorioPropietario.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar propietario: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}