using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_grupo_9.Models
{
    public class LoginView
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Usuario { get; set; } = "";

        [Required(ErrorMessage = "La clave es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = "";
    }
}