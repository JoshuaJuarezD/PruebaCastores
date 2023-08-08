using Azure;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PruebaCastores.Models;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

namespace PruebaCastores.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string connectionString = "server=localhost;database=joshua_juarez;user=prueba;password=prueba;";
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Show(int id)
        {
            NoticiaShow noticia = new NoticiaShow();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select N.titular,N.entrada,N.cuerpo,N.ruta_imagen,N.pie_imagen,N.fecha_publicacion,N.id,U.usuario,U.personal_id from noticia as N inner join usuario as U on U.id = N.usuario_id where N.id=@id";
                command.Parameters.AddWithValue("@id", id);
                MySqlDataReader lector = command.ExecuteReader();
                if (lector.Read())
                {
                    noticia.Titular = lector.GetString(0);
                    noticia.Entrada = lector.IsDBNull(1) ? "" : lector.GetString(1);
                    noticia.Cuerpo = lector.GetString(2);
                    noticia.RutaImagen = lector.IsDBNull(3) ? "" : lector.GetString(3);
                    noticia.PieImagen = lector.IsDBNull(4) ? "" : lector.GetString(4);
                    noticia.FechaPublicacion = lector.GetDateTime(5);
                    noticia.Id = lector.GetInt32(6);
                    noticia.UsuarioNombre = lector.GetString(7);
                    noticia.Id_personal = lector.IsDBNull(8) ? 0 : lector.GetInt32(8);
                }
                lector.Close();
                noticia.Comentarios=new List<Comentario>();
                command = connection.CreateCommand();
                command.CommandText = "select c.id,c.noticia_id,c.comentario,c.fecha_creacion,u.usuario,u.personal_id from comentario as c inner join usuario u on u.id=c.usuario_id inner join noticia as n on n.id= c.noticia_id where n.id = @notid ;";
                command.Parameters.AddWithValue("@notid", id);
                lector = command.ExecuteReader();
                while (lector.Read())
                {
                    Comentario aux = new Comentario();
                    aux.Id = lector.GetInt32(0);
                    aux.Noticia_Id = lector.GetInt32(1);
                    aux.Text = lector.GetString(2);
                    aux.fecha_creacion = lector.GetDateTime(3);
                    aux.NombreUsuario = lector.GetString(4);
                    aux.Id_personal = lector.IsDBNull(5) ? 0 : lector.GetInt32(5);
                    noticia.Comentarios.Add(aux);
                }
                lector.Close();
                for(int c = 0; c < noticia.Comentarios.Count(); c++)
                {
                    noticia.Comentarios[c].Respuestas = new List<Respuestas>();
                    command = connection.CreateCommand();
                    command.CommandText = "select c.id,c.comentario_id,c.respuesta,c.fecha_creacion,u.usuario,u.personal_id from Respuesta as c inner join usuario u on u.id=c.usuario_id inner join comentario as n on n.id= c.comentario_id where n.id = @comentid";
                    command.Parameters.AddWithValue("@comentid", noticia.Comentarios[c].Id);
                    lector = command.ExecuteReader();
                    while (lector.Read())
                    {
                        Respuestas aux = new Respuestas();
                        aux.Id = lector.GetInt32(0);
                        aux.Comentario_Id = lector.GetInt32(1);
                        aux.Text = lector.GetString(2);
                        aux.fecha_creacion = lector.GetDateTime(3);
                        aux.NombreUsuario = lector.GetString(4);
                        aux.Id_personal = lector.IsDBNull(5) ? 0 : lector.GetInt32(5);
                        noticia.Comentarios[c].Respuestas.Add(aux);
                    }
                    lector.Close();
                }
            }
            return View(noticia);
        }


        public IActionResult Index()
        {
            var idpersonal = 0;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT personal_id FROM usuario WHERE usuario = @usuario";
                    command.Parameters.AddWithValue("@usuario", userId);
                    MySqlDataReader lector = command.ExecuteReader();
                    if (lector.Read())
                    {
                        idpersonal = lector.IsDBNull(0) ? 0 : lector.GetInt32(0);
                    }
                    lector.Close();
                }
            }
            ViewBag.TieneIdPersonal = idpersonal;
            List<Noticia> noticias = new List<Noticia>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select  N.titular,N.entrada,N.cuerpo,N.ruta_imagen,N.pie_imagen,N.fecha_publicacion,N.id,U.usuario from noticia as N inner join usuario as U on U.id = N.usuario_id;";
                MySqlDataReader lector = command.ExecuteReader();
                while (lector.Read())
                {
                    Noticia aux = new Noticia();
                    aux.Titular = lector.GetString(0);
                    aux.Entrada = lector.IsDBNull(1) ? "" : lector.GetString(1);
                    aux.Cuerpo = lector.GetString(2);
                    aux.RutaImagen = lector.IsDBNull(3) ? "" : lector.GetString(3);
                    aux.PieImagen = lector.IsDBNull(4) ? "" : lector.GetString(4);
                    aux.FechaPublicacion = lector.GetDateTime(5);
                    aux.Id = lector.GetInt32(6);
                    aux.UsuarioNombre = lector.GetString(7);
                    noticias.Add(aux);
                }
            }
            return View(noticias);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Noticia noticia)
        {
            var userId = GetLoggedUserId();
            if (userId == null)
            {
                return RedirectToAction("", "Home");
            }
            if (noticia.Imagen != null && noticia.Imagen.Length > 0)
            {
                string nombreImagen = Guid.NewGuid().ToString() + Path.GetExtension(noticia.Imagen.FileName);
                string rutaGuardarImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", nombreImagen);
                using (var stream = new FileStream(rutaGuardarImagen, FileMode.Create))
                {
                    await noticia.Imagen.CopyToAsync(stream);
                }

                // Actualizar el campo RutaImagen en el modelo Noticia
                noticia.RutaImagen = "/imagenes/" + nombreImagen;
            }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO noticia (usuario_id, titular, entrada,cuerpo,ruta_imagen,pie_imagen,fecha_publicacion) VALUES (@usuario_id, @titular, @entrada,@cuerpo,@ruta_imagen,@pie_imagen,@fecha_publicacion)";
                command.Parameters.AddWithValue("@usuario_id", userId);
                command.Parameters.AddWithValue("@titular", noticia.Titular);
                command.Parameters.AddWithValue("@entrada", noticia.Entrada);
                command.Parameters.AddWithValue("@cuerpo", noticia.Cuerpo);
                command.Parameters.AddWithValue("@ruta_imagen", noticia.RutaImagen);
                command.Parameters.AddWithValue("@pie_imagen", noticia.PieImagen);
                command.Parameters.AddWithValue("@fecha_publicacion", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }

            return RedirectToAction("", "Home");
        }

        [HttpGet]
        public IActionResult CreatePersonal()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePersonal(Personal personal)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    int idper = 0;
                    var userId ="";
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO personal (apepaterno, apematerno,nombre,direccion,fechadeingreso) VALUES ( @apepaterno, @apematerno,@nombre,@direccion,@fechadeingreso) ";
                    command.Parameters.AddWithValue("@apepaterno", personal.apepaterno);
                    command.Parameters.AddWithValue("@apematerno", personal.apematerno);
                    command.Parameters.AddWithValue("@nombre", personal.nombre);
                    command.Parameters.AddWithValue("@direccion", personal.direccion);
                    command.Parameters.AddWithValue("@fechadeingreso", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                    command = connection.CreateCommand();
                    command.CommandText = "select idpersonal from personal where apepaterno =@apepaterno and apematerno=@apematerno ";
                    command.Parameters.AddWithValue("@apepaterno", personal.apepaterno);
                    command.Parameters.AddWithValue("@apematerno", personal.apematerno);
                    MySqlDataReader lector = command.ExecuteReader();
                    if (lector.Read())
                    {
                        idper = lector.GetInt32(0);
                    }
                    if (User.Identity.IsAuthenticated)
                    {
                        userId = User.FindFirstValue(ClaimTypes.Name);
                    }
                    lector.Close();
                    command = connection.CreateCommand();
                    command.CommandText = "update usuario set personal_id=@personal_id where usuario = @usuario";
                    command.Parameters.AddWithValue("@personal_id", idper);
                    command.Parameters.AddWithValue("@usuario",userId);
                    command.ExecuteNonQuery();

                }

                return RedirectToAction("", "Home");
            }

            return View(personal);
        }
        [HttpPost]
        public IActionResult Comentar(int noticiaId, string comentario)
        {

            var userId = GetLoggedUserId();
            if (userId == null)
            {
                return RedirectToAction("", "Home");
            }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO comentario (usuario_id, noticia_id, comentario,fecha_creacion) VALUES (@usuario_id, @noticia_id, @comentario,@fecha_creacion)";
                command.Parameters.AddWithValue("@usuario_id", userId);
                command.Parameters.AddWithValue("@noticia_id", noticiaId);
                command.Parameters.AddWithValue("@comentario", comentario);
                command.Parameters.AddWithValue("@fecha_creacion", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }
            return RedirectToAction("Show", noticiaId);
        }

        public IActionResult ResponderComentario(int comentarioId, int noticiaId, string respuesta)
        {
            var userId = GetLoggedUserId();
            if (userId == null)
            {
                return RedirectToAction("", "Home");
            }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO respuesta (usuario_id, comentario_id, respuesta,fecha_creacion) VALUES (@usuario_id, @noticia_id, @comentario,@fecha_creacion)";
                command.Parameters.AddWithValue("@usuario_id", userId);
                command.Parameters.AddWithValue("@noticia_id", noticiaId);
                command.Parameters.AddWithValue("@comentario", respuesta);
                command.Parameters.AddWithValue("@fecha_creacion", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }
            return RedirectToAction("Show", noticiaId);
        }

        private int? GetLoggedUserId()
        {
            var userName = User.Identity.Name;
            int? userid = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select id from usuario where usuario=@usuario";
                command.Parameters.AddWithValue("@usuario", userName);
                MySqlDataReader lector = command.ExecuteReader();
                if (lector.Read())
                {
                    userid = lector.GetInt32(0);
                }
            }
            return userid;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}