using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria_grupo_9.Models
{
    public enum enRoles
    {
        Administrador = 1,
        Empleado = 2,
    }

    public class Usuario
    {
        [Key]
        [Display(Name = "Código")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La clave es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = "";

        public string? Avatar { get; set; }

        [NotMapped]
        [Display(Name = "Foto de perfil")]
        public IFormFile? AvatarFile { get; set; }

        [Required]
        public int Rol { get; set; }

        [NotMapped]
        [Display(Name = "Rol")]
        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRol = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor) ?? "");
            }
            return roles;
        }
    }
}