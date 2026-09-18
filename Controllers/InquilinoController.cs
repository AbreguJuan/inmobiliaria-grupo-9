using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult Index(string busqueda, int pagina = 1)
        {
            try
            {
                const int tamPagina = 5;
                IList<Inquilino> inquilinos;

                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    inquilinos = _repositorioInquilino.Buscar(busqueda);
                    ViewBag.TotalPaginas = 1;
                }
                else
                {
                    int totalRegistros = _repositorioInquilino.ObtenerCantidad();

                    int totalPaginas = (int)Math.Ceiling(
                        (double)totalRegistros / tamPagina
                    );

                    inquilinos = _repositorioInquilino.ObtenerLista(
                        pagina,
                        tamPagina
                    );

                    ViewBag.TotalPaginas = totalPaginas;
                }

                ViewBag.PaginaActual = pagina;
                ViewBag.Busqueda = busqueda;

                return View(inquilinos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener inquilinos: {ex.Message}");
                return View(new List<Inquilino>());
            }
        }

        // GET: Inquilino/Details/5
        public IActionResult Details(int id)
        {
            var inquilino = _repositorioInquilino.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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