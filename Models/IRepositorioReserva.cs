namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioReserva : IRepositorio<Reserva>
    {
        bool ExisteSuperposicion(
            int idInmueble,
            DateTime desde,
            DateTime hasta,
            int idReservaExcluida = 0
        );

        int FinalizarReserva(
            int idReserva,
            DateTime fechaFinalizacion,
            int idUsuario
        );

        IList<Reserva> ObtenerPorInmueble(int idInmueble);
    }
}