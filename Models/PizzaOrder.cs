using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaRestaurant.Models
{
    public class PizzaOrder
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public string Toppings { get; set; }

        [Range(1, 20)]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}

