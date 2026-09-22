using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_grupo_9.Models;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;


namespace inmobiliaria_grupo_9.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorioPago;
        private readonly IRepositorioReserva repositorioReserva;
        private readonly IRepositorioInmueble repositorioInmueble;

        public PagoController(
            IRepositorioPago repositorioPago,
            IRepositorioReserva repositorioReserva,
            IRepositorioInmueble repositorioInmueble)
        {
            this.repositorioPago = repositorioPago;
            this.repositorioReserva = repositorioReserva;
            this.repositorioInmueble = repositorioInmueble;
        }

        public IActionResult Index(int pagina = 1, string? concepto = null, decimal? importeMin = null, decimal? importeMax = null,
    DateTime? fechaDesde = null, DateTime? fechaHasta = null, string? anuladoFiltro = null, string? inquilino = null)
{
            try
            {
                bool? anulado = anuladoFiltro switch
                {
                    "anulados" => true,
                    "vigentes" => false,
                    _ => null
                };

                bool hayFiltros = !string.IsNullOrWhiteSpace(concepto) || importeMin.HasValue || importeMax.HasValue
                    || fechaDesde.HasValue || fechaHasta.HasValue || anulado.HasValue || !string.IsNullOrWhiteSpace(inquilino);

                IList<Pago> lista;

                if (hayFiltros)
                {
                    lista = repositorioPago.Buscar(concepto, importeMin, importeMax, fechaDesde, fechaHasta, anulado, inquilino);
                    ViewBag.PaginaActual = 1;
                    ViewBag.TotalPaginas = 1;
                }
                else
                {
                    const int tamPagina = 5;
                    int totalRegistros = repositorioPago.ObtenerCantidad();
                    int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                    lista = repositorioPago.ObtenerLista(pagina, tamPagina);

                    ViewBag.PaginaActual = pagina;
                    ViewBag.TotalPaginas = totalPaginas;
                }

                ViewBag.Concepto = concepto;
                ViewBag.ImporteMin = importeMin;
                ViewBag.ImporteMax = importeMax;
                ViewBag.FechaDesde = fechaDesde;
                ViewBag.FechaHasta = fechaHasta;
                ViewBag.AnuladoFiltro = anuladoFiltro;
                ViewBag.Inquilino = inquilino;

                return View(lista);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener pagos: {ex.Message}");
                return View(new List<Pago>());
            }
        }

        public IActionResult Details(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        public IActionResult Create(int idReserva)
        {
            var pago = new Pago
            {
                IdReserva = idReserva,
                FechaPago = DateTime.Now
            };
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                var claimId = User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    ?.Value;

                if (claimId != null)
                {
                    pago.CreadoPor = int.Parse(claimId);
                }

                repositorioPago.Alta(pago);

                return RedirectToAction(
                    nameof(PorReserva),
                    new { idReserva = pago.IdReserva }
                );
            }
            return View(pago);
        }

        public IActionResult Edit(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, Pago pago)
{
    try
    {
        // 1. Buscamos el pago original en la base de datos
        var pagoOriginal = repositorioPago.ObtenerPorId(id);
        if (pagoOriginal == null)
        {
            return NotFound();
        }

        // 2. Solo actualizamos el concepto (ignoramos Importe y Fecha)
        pagoOriginal.Concepto = pago.Concepto;

        // 3. Limpiamos las validaciones de los campos que no nos importan
        ModelState.Remove("Importe");
        ModelState.Remove("FechaPago");
        ModelState.Remove("IdReserva");

        if (ModelState.IsValid)
        {
            repositorioPago.Modificacion(pagoOriginal);
            return RedirectToAction(nameof(Index));
        }
        
        // Si hay error, devolvemos el original para no perder los datos visuales
        return View(pagoOriginal);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al editar pago: {ex.Message}");
        return View(pago);
    }
}

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmado(int id)
        {
            var claimId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            int idUsuario = 0;
            if (claimId != null)
            {
                idUsuario = int.Parse(claimId);
            }

            repositorioPago.Anular(id, idUsuario);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult PorReserva(int idReserva)
        {
            var reserva = repositorioReserva.ObtenerPorId(idReserva);
            if (reserva == null) return NotFound();

            var inmueble = repositorioInmueble.ObtenerPorId(reserva.IdInmueble);
            if (inmueble == null) return NotFound();

            var pagos = repositorioPago.ObtenerPorReserva(idReserva);

            int cantidadDias = (reserva.Hasta.Date - reserva.Desde.Date).Days;
            decimal totalReserva = cantidadDias * reserva.MontoDiario;
            decimal importeSenia = totalReserva * inmueble.PorcentajeReserva / 100m;

            decimal totalPagado = pagos
                .Where(p => !p.Anulado && p.Concepto != "Multa por finalización anticipada")
                .Sum(p => p.Importe);

            decimal saldoPendiente = totalReserva - totalPagado;
            if (saldoPendiente < 0) saldoPendiente = 0;

            ViewBag.IdReserva = idReserva;
            ViewBag.TotalReserva = totalReserva;
            ViewBag.PorcentajeReserva = inmueble.PorcentajeReserva;
            ViewBag.ImporteSenia = importeSenia;
            ViewBag.TotalPagado = totalPagado;
            ViewBag.SaldoPendiente = saldoPendiente;

            return View(pagos);
        }
    }
}