namespace project26.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;
    using System.Security.Cryptography;
    using System.Text;

    namespace PizzaRestaurant.Controllers
    {
        public class AccountController : Controller
        {
            private readonly string _connStr;

            public AccountController(IConfiguration configuration)
            {
                _connStr = configuration.GetConnectionString("DefaultConnection");
            }

            public IActionResult Login() => View();
            public IActionResult Register() => View();

            [HttpPost]
            public IActionResult Register(string username, string password)
            {
                string hash = ComputeSha256Hash(password);

                using var conn = new MySqlConnection(_connStr);
                conn.Open();

                var cmd = new MySqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@u, @p)", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", hash);

                try
                {
                    cmd.ExecuteNonQuery();
                    ViewBag.Message = "Account created successfully!";
                }
                catch
                {
                    ViewBag.Message = "Username already exists.";
                }

                return View();
            }

            [HttpPost]
            public IActionResult Login(string username, string password)
            {
                string hash = ComputeSha256Hash(password);

                using var conn = new MySqlConnection(_connStr);
                conn.Open();

                var cmd = new MySqlCommand("SELECT UserID FROM Users WHERE Username=@u AND PasswordHash=@p", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", hash);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    HttpContext.Session.SetInt32("UserID", reader.GetInt32("UserID"));
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Message = "Invalid username or password.";
                return View();
            }

            public IActionResult Logout()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            private static string ComputeSha256Hash(string rawData)
            {
                using var sha256 = SHA256.Create();
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }

}
