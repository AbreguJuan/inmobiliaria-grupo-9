namespace inmobiliaria_grupo_9.Models
{
    public class InmuebleConteo
    {
        public int IdInmueble { get; set; }
        public string Direccion { get; set; } = "";
        public string TipoNombre { get; set; } = "";
        public int CantidadReservas { get; set; }
    }
}