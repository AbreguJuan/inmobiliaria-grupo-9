using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_grupo_9.Models;
using System.Security.Claims;


namespace inmobiliaria_grupo_9.Controllers
{
    [Authorize]
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
                .Where(i => i.Habilitado).ToList();

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "Nombre", idInquilino);
            ViewBag.Inmuebles = new SelectList(inmuebles.Select(i => new { i.IdInmueble, Texto = i.ToString() }), "IdInmueble", "Texto", idInmueble);
        }

        public IActionResult Index(
    string inquilino,
    string inmueble,
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    string finalizadaFiltro,
    int pagina = 1)
{
    try
    {
        const int tamPagina = 5;

        if (pagina < 1)
            pagina = 1;

        bool? finalizada = finalizadaFiltro switch
        {
            "finalizadas" => true,
            "vigentes" => false,
            _ => null
        };

        bool hayFiltros =
            !string.IsNullOrWhiteSpace(inquilino) ||
            !string.IsNullOrWhiteSpace(inmueble) ||
            fechaDesde.HasValue ||
            fechaHasta.HasValue ||
            finalizada.HasValue;

        List<Reserva> reservas;
        int totalPaginas;

        if (hayFiltros)
        {
            reservas = _repositorioReserva.Buscar(
                inquilino,
                inmueble,
                fechaDesde,
                fechaHasta,
                finalizada
            ).ToList();

            // Por ahora mantenemos la búsqueda actual.
            totalPaginas = 1;
        }
        else
        {
            int totalRegistros = _repositorioReserva.ObtenerCantidad();

            totalPaginas = (int)Math.Ceiling(
                totalRegistros / (double)tamPagina
            );

            if (totalPaginas > 0 && pagina > totalPaginas)
                pagina = totalPaginas;

            reservas = _repositorioReserva
                .ObtenerLista(pagina, tamPagina)
                .ToList();
        }

        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = totalPaginas;

        ViewBag.Inquilino = inquilino;
        ViewBag.Inmueble = inmueble;
        ViewBag.FechaDesde = fechaDesde;
        ViewBag.FechaHasta = fechaHasta;
        ViewBag.FinalizadaFiltro = finalizadaFiltro;

        return View(reservas);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al obtener reservas: {ex.Message}");
        return View(new List<Reserva>());
    }
}

        public IActionResult Details(int id)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            try
            {
                var inmueble = _repositorioInmueble.ObtenerPorId(reserva.IdInmueble);

                if (inmueble == null || !inmueble.Habilitado)
                {
                    ModelState.AddModelError("IdInmueble", "Ese inmueble no está disponible para reservar");
                }
                else if (reserva.Hasta <= reserva.Desde)
                {
                    ModelState.AddModelError("Hasta", "La fecha de fin debe ser posterior a la de inicio");
                }
                else if (_repositorioReserva.ExisteSuperposicion(reserva.IdInmueble, reserva.Desde, reserva.Hasta))
                {
                    ModelState.AddModelError("", "Ese inmueble ya tiene una reserva en ese rango de fechas");
                }

                if (ModelState.IsValid && inmueble != null)
                {
                    var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (claimId != null) reserva.CreadoPor = int.Parse(claimId);

                    reserva.MontoDiario = inmueble.PrecioXDia;
                    reserva.CreadoPor = ObtenerIdUsuarioActual();
                    int idReserva = _repositorioReserva.Alta(reserva);

                    int cantidadDias = (reserva.Hasta.Date - reserva.Desde.Date).Days;
                    decimal totalReserva = cantidadDias * reserva.MontoDiario;
                    decimal importeSenia = totalReserva * inmueble.PorcentajeReserva / 100m;

                    var pagoSenia = new Pago
                    {
                        IdReserva = idReserva,
                        Concepto = $"Seña de reserva ({inmueble.PorcentajeReserva:0.##}%)",
                        FechaPago = DateTime.Now,
                        Importe = importeSenia,
                        Anulado = false,
                        CreadoPor = reserva.CreadoPor
                    };

                    _repositorioPago.Alta(pagoSenia);
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

        public IActionResult Edit(int id)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            CargarListas(reserva.IdInquilino, reserva.IdInmueble);
            return View(reserva);
        }

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

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizarConfirmado(int id, DateTime fechaFinalizacion)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();

            if (fechaFinalizacion <= reserva.Desde || fechaFinalizacion >= reserva.Hasta)
            {
                ModelState.AddModelError("", "La fecha de finalización debe estar entre la fecha de inicio y la fecha de fin original.");
                return View("Finalizar", reserva);
            }

            double diasTotales = (reserva.Hasta - reserva.Desde).TotalDays;
            double diasTranscurridos = (fechaFinalizacion - reserva.Desde).TotalDays;
            double diasRestantes = (reserva.Hasta - fechaFinalizacion).TotalDays;
            decimal alquilerRestante = (decimal)diasRestantes * reserva.MontoDiario;

            decimal porcentajeMulta = diasTranscurridos < (diasTotales / 2) ? 0.50m : 0.25m;
            decimal multa = alquilerRestante * porcentajeMulta;

            ViewBag.FechaFinalizacion = fechaFinalizacion;
            ViewBag.DiasRestantes = diasRestantes;
            ViewBag.PorcentajeMulta = porcentajeMulta * 100;
            ViewBag.Multa = multa;

            return View("ConfirmarFinalizacion", reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PagarMulta(int id, DateTime fechaFinalizacion, decimal multa)
        {
            try
            {
                var reserva = _repositorioReserva.ObtenerPorId(id);
                if (reserva == null) return NotFound();
                if (reserva.Finalizada) return RedirectToAction(nameof(Index));

                var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                int idUsuario = 0;
                if (claimId != null) idUsuario = int.Parse(claimId);

                var pago = new Pago
                {
                    IdReserva = id,
                    Concepto = "Multa por finalización anticipada",
                    FechaPago = DateTime.Now,
                    Importe = multa,
                    Anulado = false,
                    CreadoPor = ObtenerIdUsuarioActual()
                };

                _repositorioPago.Alta(pago);
                _repositorioReserva.FinalizarReserva(id, fechaFinalizacion, ObtenerIdUsuarioActual());

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al finalizar reserva: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Renovar(int id)
        {
            var reserva = _repositorioReserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            if (reserva.Finalizada) return RedirectToAction(nameof(Index));
            return View(reserva);
        }
        //Metodo auxiliar que se una en create y finalizarConfirmado
        private int? ObtenerIdUsuarioActual()
        {
            var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return claimId != null ? int.Parse(claimId) : null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenovarConfirmado(int id, DateTime nuevaFechaHasta)
        {
            var reservaOriginal = _repositorioReserva.ObtenerPorId(id);

            if (reservaOriginal == null)
            {
                return NotFound();
            }

            if (reservaOriginal.Finalizada)
            {
                return RedirectToAction(nameof(Index));
            }

            if (nuevaFechaHasta <= reservaOriginal.Hasta)
            {
                ModelState.AddModelError("", "La nueva fecha de finalización debe ser posterior a la fecha actual.");
                return View("Renovar", reservaOriginal);
            }

            if (_repositorioReserva.ExisteSuperposicion(
                reservaOriginal.IdInmueble,
                reservaOriginal.Hasta,
                nuevaFechaHasta,
                reservaOriginal.IdReserva))
            {
                ModelState.AddModelError("", "No se puede renovar porque el inmueble tiene otra reserva en ese período.");
                return View("Renovar", reservaOriginal);
            }

            var inmueble = _repositorioInmueble.ObtenerPorId(reservaOriginal.IdInmueble);
            if (inmueble == null)
            {
                return NotFound();
            }

            // No modificamos la reserva original: creamos una nueva,
            // con el mismo inquilino e inmueble, continuando desde donde terminaba la anterior.

            var nuevaReserva = new Reserva
            {
                IdInquilino = reservaOriginal.IdInquilino,
                IdInmueble = reservaOriginal.IdInmueble,
                Desde = reservaOriginal.Hasta,
                Hasta = nuevaFechaHasta,
                MontoDiario = Convert.ToDecimal(inmueble.PrecioXDia), // tarifa vigente al momento de renovar
                CreadoPor = ObtenerIdUsuarioActual()
            };

           int idNuevaReserva = _repositorioReserva.Alta(nuevaReserva);

// Calculamos únicamente el período de la renovación
int cantidadDias = (nuevaReserva.Hasta.Date - nuevaReserva.Desde.Date).Days;
decimal totalRenovacion = cantidadDias * nuevaReserva.MontoDiario;

// Calculamos la seña según el porcentaje configurado en el inmueble
decimal importeSenia = totalRenovacion * inmueble.PorcentajeReserva / 100m;

// Creamos el pago de la seña para LA NUEVA reserva
var pagoSenia = new Pago
{
    IdReserva = idNuevaReserva,
    Concepto = $"Seña de reserva ({inmueble.PorcentajeReserva:0.##}%)",
    FechaPago = DateTime.Now,
    Importe = importeSenia,
    Anulado = false,
    CreadoPor = ObtenerIdUsuarioActual()
};

_repositorioPago.Alta(pagoSenia);

return RedirectToAction(nameof(Index));
        }

        public IActionResult MasReservados(int dias = 365, int top = 10)
        {
            var lista = _repositorioReserva.ObtenerMasReservados(dias, top);
            ViewBag.Dias = dias;
            return View(lista);
        }

        public IActionResult Vigentes()
        {
            var lista = _repositorioReserva.ObtenerVigentes();
            return View(lista);
        }

        public IActionResult PorVencer(int dias = 7)
        {
            var lista = _repositorioReserva.ObtenerPorVencer(dias);
            ViewBag.Dias = dias;
            return View(lista);
        }
    }
}