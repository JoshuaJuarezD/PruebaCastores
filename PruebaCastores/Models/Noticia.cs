using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaCastores.Models
{
    public class Noticia
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        public string UsuarioNombre { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener al menos {2} caracteres.", MinimumLength = 2)]
        public string Titular { get; set; }

        [StringLength(200, ErrorMessage = "El campo {0} debe tener al menos {2} caracteres.", MinimumLength = 2)]
        public string Entrada { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(500, ErrorMessage = "El campo {0} debe tener al menos {2} caracteres.", MinimumLength = 2)]
        public string Cuerpo { get; set; }
        [StringLength(200)]
        public string RutaImagen { get; set; }

        [StringLength(200)]
        public string PieImagen { get; set; }
        public DateTime FechaPublicacion { get; set; }

        public IFormFile Imagen { get; set; }
    }

    public class NoticiaShow
    {
        public int Id { get; set; }
        public string UsuarioNombre { get; set; }
        public string Titular { get; set; }
        public string Entrada { get; set; }
        public string Cuerpo { get; set; }
        public string RutaImagen { get; set; }
        public string PieImagen { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public IFormFile Imagen { get; set; }
        public int Id_personal { get; set; }
        public List<Comentario> Comentarios { get; set; }


    }
}