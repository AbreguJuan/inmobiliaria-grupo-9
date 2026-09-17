using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria_grupo_9.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "Elegí un tipo de inmueble")]
        [Display(Name = "Tipo")]
        public int IdTipoInmueble { get; set; }

        [ForeignKey(nameof(IdTipoInmueble))]
        public TipoDeInmueble? TipoDeInmueble { get; set; }

        [Required]
        public string Provincia { get; set; } = "";

        [Required]
        public string Localidad { get; set; } = "";

        [Required]
        public string Direccion { get; set; } = "";

        [Display(Name = "Precio por día")]
        public decimal PrecioXDia { get; set; }
        [Range(0, 100, ErrorMessage = "El porcentaje de reserva debe estar entre 0 y 100")]
[Display(Name = "Porcentaje de reserva")]
public decimal PorcentajeReserva { get; set; }

        [Column("Metros_Cuadrados")]
        [Display(Name = "Metros cuadrados")]
        public decimal MetrosCuadrados { get; set; }

        [Column("Nro_Ambientes")]
        [Display(Name = "Nro. de ambientes")]
        public int NroAmbientes { get; set; }

        [Column("Nro_Banios")]
        [Display(Name = "Nro. de baños")]
        public int NroBanios { get; set; }

        [Display(Name = "Propietario")]
        public int IdPropietario { get; set; }

        [ForeignKey(nameof(IdPropietario))]
        public Propietario? Propietario { get; set; }

        public bool Habilitado { get; set; } = true;

        [Display(Name = "Foto de portada")]
        public string? FotoPortada { get; set; }

        public IList<ImagenInmueble> Imagenes { get; set; } = new List<ImagenInmueble>();

        public override string ToString()
        {
            return $"{TipoDeInmueble?.Nombre} - {Direccion} ({Localidad})";
        }
    }
}