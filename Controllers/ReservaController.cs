using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using inmobiliaria_grupo_9.Models;

namespace inmobiliaria_grupo_9.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva _repositorioReserva;
        private readonly IRepositorioInquilino _repositorioInquilino;
        private readonly IRepositorioInmueble _repositorioInmueble;

        public ReservaController(
            IRepositorioReserva repositorioReserva,
            IRepositorioInquilino repositorioInquilino,
            IRepositorioInmueble repositorioInmueble)
        {
            _repositorioReserva = repositorioReserva;
            _repositorioInquilino = repositorioInquilino;
            _repositorioInmueble = repositorioInmueble;
        }

        private void CargarListas(int? idInquilino = null, int? idInmueble = null)
        {
            var inquilinos = _repositorioInquilino.ObtenerLista(1, 1000);
            var inmuebles = _repositorioInmueble.ObtenerLista(1, 1000);

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "Nombre", idInquilino);
            // Mostramos "Tipo - Dirección" gracias al ToString() de Inmueble
            ViewBag.Inmuebles = new SelectList(inmuebles.Select(i => new { i.IdInmueble, Texto = i.ToString() }), "IdInmueble", "Texto", idInmueble);
        }

        // GET: Reserva
        public IActionResult Index()
        {
            try
            {
                var reservas = _repositorioReserva.ObtenerLista();
                return View(reservas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener reservas: {ex.Message}");
                return View(new List<Reserva>());
            }
        }

        // GET: Reserva/Details/5
        public IActionResult Details(int id)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        // GET: Reserva/Create
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        // POST: Reserva/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            try
            {
                if (reserva.Hasta <= reserva.Desde)
                {
                    ModelState.AddModelError("Hasta", "La fecha de fin debe ser posterior a la de inicio");
                }
                else if (_repositorioReserva.ExisteSuperposicion(reserva.IdInmueble, reserva.Desde, reserva.Hasta))
                {
                    ModelState.AddModelError("", "Ese inmueble ya tiene una reserva en ese rango de fechas");
                }

                if (ModelState.IsValid)
                {
                    _repositorioReserva.Alta(reserva);
                    return RedirectToAction(nameof(Index));
                }
                CargarListas(reserva.IdInquilino, reserva.IdInmueble);
                return View(reserva);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear reserva: {ex.Message}");
                CargarListas(reserva.IdInquilino, reserva.IdInmueble);
                return View(reserva);
            }
        }

        // GET: Reserva/Edit/5
        public IActionResult Edit(int id)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            CargarListas(reserva.IdInquilino, reserva.IdInmueble);
            return View(reserva);
        }

        // POST: Reserva/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva)
        {
            try
            {
                reserva.IdReserva = id;

                if (reserva.Hasta <= reserva.Desde)
                {
                    ModelState.AddModelError("Hasta", "La fecha de fin debe ser posterior a la de inicio");
                }
                else if (_repositorioReserva.ExisteSuperposicion(reserva.IdInmueble, reserva.Desde, reserva.Hasta, id))
                {
                    ModelState.AddModelError("", "Ese inmueble ya tiene una reserva en ese rango de fechas");
                }

                if (ModelState.IsValid)
                {
                    _repositorioReserva.Modificacion(reserva);
                    return RedirectToAction(nameof(Index));
                }
                CargarListas(reserva.IdInquilino, reserva.IdInmueble);
                return View(reserva);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar reserva: {ex.Message}");
                CargarListas(reserva.IdInquilino, reserva.IdInmueble);
                return View(reserva);
            }
        }

        // GET: Reserva/Delete/5
        public IActionResult Delete(int id)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorioReserva.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar reserva: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}