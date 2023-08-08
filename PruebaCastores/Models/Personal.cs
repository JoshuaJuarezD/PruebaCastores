using System.ComponentModel.DataAnnotations;

namespace PruebaCastores.Models
{
    public class Personal
    {
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string apepaterno { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string apematerno { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string nombre { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string direccion { get; set; }
        public DateTime fechadeingreso { get; set; }
    }
}
