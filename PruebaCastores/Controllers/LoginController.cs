using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PruebaCastores.Models;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PruebaCastores.Controllers
{
    public class LoginController : Controller
    {
        private readonly string connectionString = "server=localhost;database=joshua_juarez;user=prueba;password=prueba;";
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login usuario)
        {
            Usuario usuariolog = ValidarCredenciales(usuario.UsuarioNombre, usuario.Contraseña);

            if (usuariolog != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuariolog.UsuarioNombre),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("","Home");
            }
            else
            {
                ModelState.AddModelError("", "Credenciales no válidas. Inténtalo de nuevo.");
            }

            return View();
        }



        private Usuario ValidarCredenciales(string usuario, string contraseña)
        {
            Usuario response = new Usuario();
            string contraseñaEncriptadaIngresada = EncriptarSHA256(contraseña);

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM usuario WHERE usuario = @usuario";
                command.Parameters.AddWithValue("@usuario", usuario);
                MySqlDataReader lector = command.ExecuteReader();
                if (lector.Read())
                {
                    response.Id = lector.GetInt32(0);
                    response.Correo = lector.IsDBNull(1) ? "" : lector.GetString(1);
                    response.UsuarioNombre = lector.GetString(2);
                    response.Contraseña = lector.IsDBNull(3) ? "" : lector.GetString(3);
                    response.personal_id = lector.IsDBNull(4) ? null : lector.GetInt32(4);
                }
                lector.Close();
                if(contraseñaEncriptadaIngresada == response.Contraseña)
                {
                    return response;
                }
                else
                {
                    return null;
                }
            }

            return null;
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
