namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioPago
    {
        int Alta(Pago pago);

        int Modificacion(Pago pago);

        int Anular(int idPago, int idUsuario);

        IList<Pago> ObtenerLista(
            int paginaNro = 1,
            int tamPagina = 10
        );

        int ObtenerCantidad();

        Pago? ObtenerPorId(int idPago);

        IList<Pago> ObtenerPorReserva(int idReserva);

        IList<Pago> Buscar(string? concepto = null, decimal? importeMin = null, decimal? importeMax = null,
            DateTime? fechaDesde = null, DateTime? fechaHasta = null, bool? anulado = null, string? inquilino = null);
    }
}