using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_grupo_9.Models
{
    public class Propietario
    {
        [Key]
        [Display(Name = "Código")]
        public int IdPropietario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string Dni { get; set; } = "";

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La clave es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = "";

        public override string ToString()
        {
            var res = $"{Nombre} {Apellido}";
            if(!String.IsNullOrEmpty(Dni)) {
                res += $" ({Dni})";
            }
            return res;
        }
    }
}