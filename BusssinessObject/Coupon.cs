using System;
using System.Collections.Generic;

namespace BusssinessObject;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string Code { get; set; } = null!;

    public decimal? DiscountAmount { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal? MaxDiscount { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
