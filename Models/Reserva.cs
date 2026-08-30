using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_grupo_9.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "Código")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Elegí un inquilino")]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [Required(ErrorMessage = "Elegí un inmueble")]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Desde { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Hasta { get; set; }

        // Nullable porque Pago todavía no existe cuando se crea la reserva
        [Display(Name = "Pago")]
        public int? IdPago { get; set; }
    }
}