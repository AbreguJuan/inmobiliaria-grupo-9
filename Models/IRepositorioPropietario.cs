namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        Propietario? ObtenerPorEmail(string email);
        IList<Propietario> BuscarPorNombre(string nombre);
    }
}