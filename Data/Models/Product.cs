using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }

        public int? SellerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Price { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        [RegularExpression("New|Like_New|Good|Fair")]
        public string Condition { get; set; } = string.Empty;
        public bool IsNew { get; set; } = true;
        public bool IsSale { get; set; } = false;
        public bool IsReview { get; set; } = false;
        public int? SalePrice { get; set; }
        [Required]
        public string MainImage { get; set; } = string.Empty;

        public DateTime AddedDate { get; set; } = default;

        public bool IsAvailable { get; set; } = true;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        [ForeignKey("SellerId")]
        public virtual User? Seller { get; set; }

        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
