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
    }
}