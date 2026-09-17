namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioPago
    {
        int Alta(Pago pago);

        int Modificacion(Pago pago);

        int Anular(int idPago, int idUsuario);

        IList<Pago> ObtenerLista();

        Pago? ObtenerPorId(int idPago);

        IList<Pago> ObtenerPorReserva(int idReserva);
    }
}