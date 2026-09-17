using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;
using System.Security.Claims;

namespace inmobiliaria_grupo_9.Controllers
{
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorioPago;

        public PagoController(IRepositorioPago repositorioPago)
        {
            this.repositorioPago = repositorioPago;
        }

        public IActionResult Index()
        {
            var lista = repositorioPago.ObtenerLista();
            return View(lista);
        }

        public IActionResult Details(int id)
{
    var pago = repositorioPago.ObtenerPorId(id);

    if (pago == null)
    {
        return NotFound();
    }

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
                // Capturamos el ID del usuario logueado para la auditoría (CreadoPor)
                var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (claimId != null) 
                {
                    pago.CreadoPor = int.Parse(claimId);
                }

                repositorioPago.Alta(pago);
                return RedirectToAction(nameof(Index));
            }

            return View(pago);
        }

        public IActionResult Edit(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);

            if (pago == null)
            {
                return NotFound();
            }

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

        public IActionResult Delete(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);

            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmado(int id)
        {
            // Capturamos el ID del usuario logueado para la auditoría (AnuladoPor)
            var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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
            var pagos = repositorioPago.ObtenerPorReserva(idReserva);

            ViewBag.IdReserva = idReserva;

            return View(pagos);
        }
    }
}