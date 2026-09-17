using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using inmobiliaria_grupo_9.Models;
using System.Security.Claims;


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
            var inmuebles = _repositorioInmueble.ObtenerLista(1, 1000)
                .Where(i => i.Habilitado)
                .ToList();

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "Nombre", idInquilino);
            // Mostramos "Tipo - Dirección" gracias al ToString() de Inmueble
            ViewBag.Inmuebles = new SelectList(inmuebles.Select(i => new { i.IdInmueble, Texto = i.ToString() }), "IdInmueble", "Texto", idInmueble);
        }

        // GET: Reserva
       public IActionResult Index(int pagina = 1)
{
    try
    {
        const int tamPagina = 5;

        int totalRegistros = _repositorioReserva.ObtenerCantidad();

        int totalPaginas = (int)Math.Ceiling(
            (double)totalRegistros / tamPagina
        );

        var reservas = _repositorioReserva.ObtenerLista(
            pagina,
            tamPagina
        );

        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = totalPaginas;

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
        var inmueble =
            _repositorioInmueble.ObtenerPorId(reserva.IdInmueble);

        if (inmueble == null || !inmueble.Habilitado)
        {
            ModelState.AddModelError(
                "IdInmueble",
                "Ese inmueble no está disponible para reservar"
            );
        }
        else if (reserva.Hasta <= reserva.Desde)
        {
            ModelState.AddModelError(
                "Hasta",
                "La fecha de fin debe ser posterior a la de inicio"
            );
        }
        else if (_repositorioReserva.ExisteSuperposicion(
            reserva.IdInmueble,
            reserva.Desde,
            reserva.Hasta))
        {
            ModelState.AddModelError(
                "",
                "Ese inmueble ya tiene una reserva en ese rango de fechas"
            );
        }

        if (ModelState.IsValid && inmueble != null)
        {
            // Usuario que crea la reserva
            var claimId = User.Claims
                .FirstOrDefault(
                    c => c.Type == ClaimTypes.NameIdentifier
                )?.Value;

            if (claimId != null)
            {
                reserva.CreadoPor = int.Parse(claimId);
            }

            // Guardamos el precio por día al momento de reservar
            reserva.MontoDiario = inmueble.PrecioXDia;

            // Creamos la reserva
            int idReserva =
                _repositorioReserva.Alta(reserva);

            // Calculamos el total
            int cantidadDias =
                (reserva.Hasta.Date - reserva.Desde.Date).Days;

            decimal totalReserva =
                cantidadDias * reserva.MontoDiario;

            // Calculamos la seña
            decimal importeSenia =
                totalReserva *
                inmueble.PorcentajeReserva / 100m;

            // Registramos la seña en Pagos
            var pagoSenia = new Pago
            {
                IdReserva = idReserva,
                Concepto =
                    $"Seña de reserva ({inmueble.PorcentajeReserva:0.##}%)",
                FechaPago = DateTime.Now,
                Importe = importeSenia,
                Anulado = false,
                CreadoPor = reserva.CreadoPor
            };

            _repositorioPago.Alta(pagoSenia);

            return RedirectToAction(nameof(Index));
        }

        CargarListas(
            reserva.IdInquilino,
            reserva.IdInmueble
        );

        return View(reserva);
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"Error al crear reserva: {ex.Message}"
        );

        CargarListas(
            reserva.IdInquilino,
            reserva.IdInmueble
        );

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

               // Capturamos el usuario que realiza la operación
var claimId = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

int idUsuario = 0;

if (claimId != null)
{
    idUsuario = int.Parse(claimId);
}

// Registramos la multa como un pago de la reserva
var pago = new Pago
{
    IdReserva = id,
    Concepto = "Multa por finalización anticipada",
    FechaPago = DateTime.Now,
    Importe = multa,
    Anulado = false,
    CreadoPor = idUsuario > 0 ? idUsuario : null
};

_repositorioPago.Alta(pago);

// Recién después de registrar el pago finalizamos la reserva
_repositorioReserva.FinalizarReserva(
    id,
    fechaFinalizacion,
    idUsuario
);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al finalizar reserva: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Reserva/Renovar/5
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
public IActionResult RenovarConfirmado(
    int id,
    DateTime nuevaFechaHasta)
{
    try
    {
        var reservaOriginal =
            _repositorioReserva.ObtenerPorId(id);

        if (reservaOriginal == null)
        {
            return NotFound();
        }

        if (reservaOriginal.Finalizada)
        {
            return RedirectToAction(nameof(Index));
        }

        // La nueva reserva comienza cuando termina la original
        DateTime nuevaFechaDesde = reservaOriginal.Hasta;

        if (nuevaFechaHasta <= nuevaFechaDesde)
        {
            ModelState.AddModelError(
                "",
                "La nueva fecha de finalización debe ser posterior a la fecha de finalización de la reserva original."
            );

            return View("Renovar", reservaOriginal);
        }

        // Verificamos que no exista otra reserva
        // para el inmueble durante el nuevo período
        if (_repositorioReserva.ExisteSuperposicion(
            reservaOriginal.IdInmueble,
            nuevaFechaDesde,
            nuevaFechaHasta,
            reservaOriginal.IdReserva))
        {
            ModelState.AddModelError(
                "",
                "No se puede renovar porque el inmueble tiene otra reserva en ese período."
            );

            return View("Renovar", reservaOriginal);
        }

        var inmueble =
            _repositorioInmueble.ObtenerPorId(
                reservaOriginal.IdInmueble
            );

        if (inmueble == null || !inmueble.Habilitado)
        {
            ModelState.AddModelError(
                "",
                "El inmueble no se encuentra disponible para renovar."
            );

            return View("Renovar", reservaOriginal);
        }

        // Usuario que realiza la renovación
        var claimId = User.Claims
            .FirstOrDefault(
                c => c.Type == ClaimTypes.NameIdentifier
            )?.Value;

        int? idUsuario = null;

        if (claimId != null)
        {
            idUsuario = int.Parse(claimId);
        }

        // IMPORTANTE:
        // Creamos una reserva NUEVA.
        // La reserva original NO se modifica.
        var nuevaReserva = new Reserva
        {
            IdInquilino = reservaOriginal.IdInquilino,
            IdInmueble = reservaOriginal.IdInmueble,

            Desde = nuevaFechaDesde,
            Hasta = nuevaFechaHasta,

            // Precio actual del inmueble
            MontoDiario = inmueble.PrecioXDia,

            Finalizada = false,
            FechaFinalizacion = null,

            CreadoPor = idUsuario
        };

        int nuevaReservaId =
            _repositorioReserva.Alta(nuevaReserva);

        if (nuevaReservaId <= 0)
        {
            ModelState.AddModelError(
                "",
                "No se pudo generar la nueva reserva."
            );

            return View("Renovar", reservaOriginal);
        }

        // Calculamos el total de la nueva reserva
        int cantidadDias =
            (nuevaReserva.Hasta.Date -
             nuevaReserva.Desde.Date).Days;

        decimal totalReserva =
            cantidadDias * nuevaReserva.MontoDiario;

        // Calculamos la seña de la nueva reserva
        decimal importeSenia =
            totalReserva *
            inmueble.PorcentajeReserva / 100m;

        // Registramos automáticamente la seña
        var pagoSenia = new Pago
        {
            IdReserva = nuevaReservaId,

            Concepto =
                $"Seña de reserva ({inmueble.PorcentajeReserva:0.##}%)",

            FechaPago = DateTime.Now,
            Importe = importeSenia,
            Anulado = false,
            CreadoPor = idUsuario
        };

        _repositorioPago.Alta(pagoSenia);

        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"Error al renovar reserva: {ex.Message}"
        );

        var reserva =
            _repositorioReserva.ObtenerPorId(id);

        if (reserva == null)
        {
            return NotFound();
        }

        ModelState.AddModelError(
            "",
            "Ocurrió un error al renovar la reserva."
        );

        return View("Renovar", reserva);
    }
}
    }
}
