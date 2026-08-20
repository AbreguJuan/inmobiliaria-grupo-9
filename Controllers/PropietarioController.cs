using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;
using System.Collections.Generic;
using System;

namespace inmobiliaria_grupo_9.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario _repositorioPropietario;

        // Inyección de dependencias
        public PropietarioController(IRepositorioPropietario repositorioPropietario)
        {
            _repositorioPropietario = repositorioPropietario;
        }

        // GET: Propietario
        public IActionResult Index()
        {
            try
            {
                var propietarios = _repositorioPropietario.ObtenerLista();
                return View(propietarios);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener propietarios: {ex.Message}");
                return View(new List<Propietario>());
            }
        }

        // GET: Propietario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
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

        // GET: Propietario/Edit/5
        public IActionResult Edit(int id)
        {
            var propietario = _repositorioPropietario.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietario/Edit/5
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

        // GET: Propietario/Delete/5
        public IActionResult Delete(int id)
        {
            var propietario = _repositorioPropietario.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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