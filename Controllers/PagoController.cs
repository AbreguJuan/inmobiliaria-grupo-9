using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria_grupo_9.Controllers
{
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

        public IActionResult Index(int pagina = 1)
        {
            try
            {
                const int tamPagina = 5;

                int totalRegistros = repositorioPago.ObtenerCantidad();

                int totalPaginas = (int)Math.Ceiling(
                    (double)totalRegistros / tamPagina
                );

                var lista = repositorioPago.ObtenerLista(pagina, tamPagina);

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;

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
        public IActionResult Edit(Pago pago)
        {
            if (ModelState.IsValid)
            {
                repositorioPago.Modificacion(pago);
                return RedirectToAction(nameof(Index));
            }
            return View(pago);
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