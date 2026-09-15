using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_grupo_9.Models
{
    public class TipoDeInmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdTipoInmueble { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Tipo")]
        public string Nombre { get; set; } = "";

        public bool Habilitado { get; set; } = true;

        public override string ToString() => Nombre;
    }
}