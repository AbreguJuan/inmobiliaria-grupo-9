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

        // --- Nuevos campos de Auditoría ---
        public int? CreadoPor { get; set; }
        public int? AnuladoPor { get; set; }

        // --- Propiedades de navegación ---
        public Reserva? Reserva { get; set; }
        
        // Navegación para Auditoría
        public Usuario? Creador { get; set; }
        public Usuario? Anulador { get; set; }
    }
}