using System.ComponentModel.DataAnnotations;

namespace PruebaCastores.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string UsuarioNombre { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string Contraseña { get; set; }
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContraseña { get; set; }

        [EmailAddress]
        public string Correo { get; set; }

        public int? personal_id { get; set; }
    }
}
