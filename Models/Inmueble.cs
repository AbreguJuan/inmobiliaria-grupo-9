using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria_grupo_9.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdInmueble { get; set; }

        [Required]
        public string Tipo { get; set; } = "";

        [Required]
        public string Provincia { get; set; } = "";

        [Required]
        public string Localidad { get; set; } = "";

        [Required]
        public string Direccion { get; set; } = "";

        [Display(Name = "Precio por día")]
        public decimal PrecioXDia { get; set; }

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

        public override string ToString()
        {
            return $"{Tipo} - {Direccion} ({Localidad})";
        }
    }
}