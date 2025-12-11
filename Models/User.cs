namespace project26.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace PizzaRestaurant.Models
    {
        public class User
        {
            public int UserID { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            public string PasswordHash { get; set; }
        }
    }

}
