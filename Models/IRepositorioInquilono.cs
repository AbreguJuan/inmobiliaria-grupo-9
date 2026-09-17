namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        IList<Inquilino> Buscar(string texto);
    }
}