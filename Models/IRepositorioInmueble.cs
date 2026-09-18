using System.Collections.Generic;

namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> BuscarPorPropietario(int idPropietario);
        IList<Inmueble> Buscar(string? texto, decimal? precio = null, string? operadorPrecio = null, bool? habilitado = null,
            int? ambientesMinimo = null, decimal? metrosMinimo = null, decimal? metrosMaximo = null,
            DateTime? disponibleDesde = null, DateTime? disponibleHasta = null,
            int? cupoMinimo = null, decimal? latitud = null, decimal? longitud = null, decimal? radioKm = null,
            int? idPropietario = null);
    }
}