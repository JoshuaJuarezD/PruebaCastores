using System.ComponentModel.DataAnnotations;

namespace PruebaCastores.Models
{
    public class Login
    {

        [StringLength(50, MinimumLength = 2)]
        public string UsuarioNombre { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string Contraseña { get; set; }

    }
}
