using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;
    [Required]
    [MaxLength(50)]
    public string FullName { get; set; } = string.Empty;


    [Required]
    [Column(TypeName = "int")]
    public int TotalAmount { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(20)")]
    [RegularExpression("Pending|Shipped|Delivered|Cancelled|Returned")]
    public string Status { get; set; } = string.Empty;

    public int? CouponId { get; set; }

    [Required]
    [MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Descriptions { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;

    [ForeignKey("CouponId")]
    public virtual Coupon? Coupon { get; set; }

    [ForeignKey("CustomerId")]
    public virtual User? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
