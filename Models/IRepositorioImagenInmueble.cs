namespace inmobiliaria_grupo_9.Models
{
    public interface IRepositorioImagenInmueble
    {
        int Alta(ImagenInmueble imagen);
        int Baja(int idImagen);
        IList<ImagenInmueble> ObtenerPorInmueble(int idInmueble);
    }
}