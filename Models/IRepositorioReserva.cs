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

        int FinalizarReserva(int idReserva, DateTime fechaFinalizacion, int? terminadoPor);

        IList<Reserva> ObtenerPorInmueble(int idInmueble);
        IList<InmuebleConteo> ObtenerMasReservados(int dias = 365, int top = 10);
        IList<Reserva> ObtenerVigentes();
        IList<Reserva> ObtenerPorVencer(int dias);

        IList<Reserva> Buscar(string? inquilino = null, string? inmueble = null,
            DateTime? fechaDesde = null, DateTime? fechaHasta = null, bool? finalizada = null);
    }
}