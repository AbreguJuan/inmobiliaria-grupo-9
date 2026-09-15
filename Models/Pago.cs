using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_grupo_9.Models
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [Required]
        public int IdReserva { get; set; }

        [Required]
        [Display(Name = "Concepto")]
        public string Concepto { get; set; } = "";

        [Required]
        [Display(Name = "Fecha de pago")]
        public DateTime FechaPago { get; set; }

        [Required]
        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        public bool Anulado { get; set; }

        public Reserva? Reserva { get; set; }
    }
}