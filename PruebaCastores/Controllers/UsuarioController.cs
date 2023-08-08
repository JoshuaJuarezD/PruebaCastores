using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PruebaCastores.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol.Plugins;

namespace PruebaCastores.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly string connectionString = "server=localhost;database=joshua_juarez;user=prueba;password=prueba;";

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Contraseña = EncriptarSHA256(usuario.Contraseña);

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO usuario (correo, usuario, contraseña) VALUES (@correo, @usuario, @contraseña)";
                    command.Parameters.AddWithValue("@usuario", usuario.UsuarioNombre);
                    command.Parameters.AddWithValue("@contraseña", usuario.Contraseña);
                    command.Parameters.AddWithValue("@correo", usuario.Correo);
                    command.ExecuteNonQuery();
                }

                return RedirectToAction("", "Home");
            }

            return View(usuario);
        }
        private string EncriptarSHA256(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
