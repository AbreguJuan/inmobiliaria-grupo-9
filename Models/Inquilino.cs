using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_grupo_9.Models
{
    public class Inquilino
    {
        [Key]
        public int IdInquilino { get; set; }

        [Required]
        public string Dni { get; set; } = "";

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        public string? Telefono { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}