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
        [Display(Name = "Desde")]
        public DateTime Desde { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Hasta")]
        public DateTime Hasta { get; set; }

        [Display(Name = "Fecha de finalización")]
        public DateTime? FechaFinalizacion { get; set; }

        [Display(Name = "Finalizada")]
        public bool Finalizada { get; set; }

        [Display(Name = "Monto diario")]
        public decimal MontoDiario { get; set; }

        // --- Nuevos campos de Auditoría ---
        public int? CreadoPor { get; set; }
        public int? TerminadoPor { get; set; }

        // --- Propiedades de navegación ---
        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }
        
        // Navegación para Auditoría
        public Usuario? Creador { get; set; }
        public Usuario? Terminador { get; set; }
    }
}