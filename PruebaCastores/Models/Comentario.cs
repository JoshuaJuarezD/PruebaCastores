namespace PruebaCastores.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        public int Usuario_Id { get; set; }
        
        public int Id_personal { get; set; }
        public int Noticia_Id { get; set; }
        public string Text { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string NombreUsuario { get; set; }

        public List<Respuestas> Respuestas { get; set; }
    }
}
