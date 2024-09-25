using System;
using System.Collections.Generic;

namespace BusssinessObject;

public partial class Product
{
    public int ProductId { get; set; }

    public int? SellerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public decimal Price { get; set; }

    public int? Quantity { get; set; }

    public string? Condition { get; set; }

    public DateTime? AddedDate { get; set; }

    public bool? IsAvailable { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? Seller { get; set; }

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
