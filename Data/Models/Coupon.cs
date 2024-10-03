using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public partial class Coupon
{
    [Key]
    public int CouponId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Column(TypeName = "decimal(4,2)")]
    public decimal DiscountPercentage { get; set; } = 0;

    public int MaxDiscount { get; set; } = 0;

    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int MinSpendAmount { get; set; } = 0;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
