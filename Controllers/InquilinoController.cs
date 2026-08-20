using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;
using System.Collections.Generic;
using System;

namespace inmobiliaria_grupo_9.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IRepositorioInquilino _repositorioInquilino;

        // Inyección de dependencias
        public InquilinoController(IRepositorioInquilino repositorioInquilino)
        {
            _repositorioInquilino = repositorioInquilino;
        }

        // GET: Inquilino
        public IActionResult Index()
        {
            try
            {
                var inquilinos = _repositorioInquilino.ObtenerLista();
                return View(inquilinos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener inquilinos: {ex.Message}");
                return View(new List<Inquilino>());
            }
        }

        // GET: Inquilino/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquilino/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioInquilino.Alta(inquilino);
                    return RedirectToAction(nameof(Index));
                }

                return View(inquilino);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear inquilino: {ex.Message}");
                return View(inquilino);
            }
        }

        // GET: Inquilino/Edit/5
        public IActionResult Edit(int id)
        {
            var inquilino = _repositorioInquilino.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        // POST: Inquilino/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    inquilino.IdInquilino = id;
                    _repositorioInquilino.Modificacion(inquilino);

                    return RedirectToAction(nameof(Index));
                }

                return View(inquilino);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar inquilino: {ex.Message}");
                return View(inquilino);
            }
        }

        // GET: Inquilino/Delete/5
        public IActionResult Delete(int id)
        {
            var inquilino = _repositorioInquilino.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        // POST: Inquilino/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorioInquilino.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar inquilino: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}