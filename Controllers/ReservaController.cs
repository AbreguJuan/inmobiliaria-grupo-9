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

        private readonly IRepositorioPago _repositorioPago;

       public ReservaController(
    IRepositorioReserva repositorioReserva,
    IRepositorioInquilino repositorioInquilino,
    IRepositorioInmueble repositorioInmueble,
    IRepositorioPago repositorioPago)
{
    _repositorioReserva = repositorioReserva;
    _repositorioInquilino = repositorioInquilino;
    _repositorioInmueble = repositorioInmueble;
    _repositorioPago = repositorioPago;
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
    var inmueble = _repositorioInmueble.ObtenerPorId(reserva.IdInmueble);

    if (inmueble == null)
    {
        ModelState.AddModelError("", "No se encontró el inmueble seleccionado");
        CargarListas(reserva.IdInquilino, reserva.IdInmueble);
        return View(reserva);
    }

    reserva.MontoDiario = Convert.ToDecimal(inmueble.PrecioXDia);

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
        public IActionResult Finalizar(int id)
{
             var reserva = _repositorioReserva.ObtenerPorId(id);

             if (reserva == null)
            {
              return NotFound();
            }

            return View(reserva);
}

         [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult FinalizarConfirmado(int id, DateTime fechaFinalizacion)
{
    var reserva = _repositorioReserva.ObtenerPorId(id);

    if (reserva == null)
    {
        return NotFound();
    }

    if (fechaFinalizacion <= reserva.Desde ||
        fechaFinalizacion >= reserva.Hasta)
    {
        ModelState.AddModelError(
            "",
            "La fecha de finalización debe estar entre la fecha de inicio y la fecha de fin original."
        );

        return View("Finalizar", reserva);
    }

    double diasTotales = (reserva.Hasta - reserva.Desde).TotalDays;
    double diasTranscurridos = (fechaFinalizacion - reserva.Desde).TotalDays;
    double diasRestantes = (reserva.Hasta - fechaFinalizacion).TotalDays;
    decimal alquilerRestante =
        (decimal)diasRestantes * reserva.MontoDiario;

    decimal porcentajeMulta;


    if (diasTranscurridos < diasTotales / 2)
    {
        porcentajeMulta = 0.50m;
    }
    else
    {
        porcentajeMulta = 0.25m;
    }

    decimal multa = alquilerRestante * porcentajeMulta;

    ViewBag.FechaFinalizacion = fechaFinalizacion;
    ViewBag.DiasRestantes = diasRestantes;
    ViewBag.PorcentajeMulta = porcentajeMulta * 100;
    ViewBag.Multa = multa;

    return View("ConfirmarFinalizacion", reserva);
}
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult PagarMulta(
    int id,
    DateTime fechaFinalizacion,
    decimal multa)
{
    try
    {
        var reserva = _repositorioReserva.ObtenerPorId(id);

        if (reserva == null)
        {
            return NotFound();
        }

        if (reserva.Finalizada)
        {
            return RedirectToAction(nameof(Index));
        }

        // Registramos la multa como un pago de la reserva
        var pago = new Pago
        {
            IdReserva = id,
            Concepto = "Multa por finalización anticipada",
            FechaPago = DateTime.Now,
            Importe = multa,
            Anulado = false
        };

        _repositorioPago.Alta(pago);

        // Recién después de registrar el pago,
        // finalizamos la reserva.
        _repositorioReserva.FinalizarReserva(id, fechaFinalizacion);

        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al finalizar reserva: {ex.Message}");
        return RedirectToAction(nameof(Index));
    }
}


// GET: Reserva/Renovar/5
public IActionResult Renovar(int id)
{
    var reserva = _repositorioReserva.ObtenerPorId(id);

    if (reserva == null)
    {
        return NotFound();
    }

    if (reserva.Finalizada)
    {
        return RedirectToAction(nameof(Index));
    }

    return View(reserva);
}


// POST: Reserva/Renovar
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult RenovarConfirmado(int id, DateTime nuevaFechaHasta)
{
    var reserva = _repositorioReserva.ObtenerPorId(id);

    if (reserva == null)
    {
        return NotFound();
    }

    if (reserva.Finalizada)
    {
        return RedirectToAction(nameof(Index));
    }

    // La nueva fecha tiene que ser posterior a la fecha Hasta actual
    if (nuevaFechaHasta <= reserva.Hasta)
    {
        ModelState.AddModelError(
            "",
            "La nueva fecha de finalización debe ser posterior a la fecha actual."
        );

        return View("Renovar", reserva);
    }

    // Controlamos que la extensión no se superponga
    // con otra reserva del mismo inmueble
    if (_repositorioReserva.ExisteSuperposicion(
        reserva.IdInmueble,
        reserva.Hasta,
        nuevaFechaHasta,
        reserva.IdReserva))
    {
        ModelState.AddModelError(
            "",
            "No se puede renovar porque el inmueble tiene otra reserva en ese período."
        );

        return View("Renovar", reserva);
    }

    _repositorioReserva.RenovarReserva(id, nuevaFechaHasta);

    return RedirectToAction(nameof(Index));
}
}
}
