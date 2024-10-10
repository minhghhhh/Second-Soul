using Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;

namespace Second_Soul.ViewModel
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Product code is required.")]
        public string id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public int amount { get; set; }
        public string description { get; set; }
        [Required(ErrorMessage = "Item is required.")]
        public List<Product> items { get; set; }
        public string cancelUrl { get; set; }
        public string returnUrl { get; set; }   

    }
}
