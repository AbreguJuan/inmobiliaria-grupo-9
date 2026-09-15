using Microsoft.AspNetCore.Mvc;
using inmobiliaria_grupo_9.Models;

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
            repositorioPago.Anular(id);

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

