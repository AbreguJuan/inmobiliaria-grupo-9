namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioTipoDeInmueble : IRepositorio<TipoDeInmueble>
    {
        // Para no permitir borrar un tipo que algún inmueble está usando
        int ContarInmueblesQueLoUsan(int idTipoInmueble);
    }
}