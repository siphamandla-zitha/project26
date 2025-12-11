using Microsoft.AspNetCore.Mvc;
using PizzaRestaurant.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using project26.Controllers;  

namespace PizzaRestaurant.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connStr;

        public HomeController(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Index(string size, string[] toppings, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            decimal basePrice = size switch
            {
                "Small" => 8.00m,
                "Medium" => 10.00m,
                "Large" => 12.00m,
                _ => 8.00m
            };
            decimal toppingPrice = toppings.Length * 1.50m;
            decimal total = (basePrice + toppingPrice) * quantity;

            string toppingList = string.Join(", ", toppings);

            MySqlConnection conn = new MySqlConnection(_connStr);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(@"INSERT INTO pizzaorders 
                (Size, Toppings, Quantity, TotalPrice)
                VALUES (@s, @t, @q, @p)", conn);
            cmd.Parameters.AddWithValue("@s", size);
            cmd.Parameters.AddWithValue("@t", toppingList);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.Parameters.AddWithValue("@p", total);
            cmd.ExecuteNonQuery();

            ViewBag.Message = $"Order placed successfully! Total: ${total:F2}";
            return View();
        }

        public IActionResult OrderHistory()
        {
            int? orderId = HttpContext.Session.GetInt32("OrderID");
            if (orderId == null) return RedirectToAction("Login", "Account");

            var orders = new List<PizzaOrder>();
            using var conn = new MySqlConnection(_connStr);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM pizzaorders WHERE UserID=@u ORDER BY OrderDate DESC", conn);
            cmd.Parameters.AddWithValue("@o", orderId);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["OrderID"]);
                Console.WriteLine(reader["Size"]);
                Console.WriteLine(reader["Toppings"]);
                Console.WriteLine(reader["Quantity"]);
                Console.WriteLine(reader["TotalPrice"]);

                
            }
            return View(orders);
        }
    }
}
