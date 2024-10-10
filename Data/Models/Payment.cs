using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Required]
    public int OrderId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Required]
    [Column(TypeName = "int")]
    public int Amount { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    [RegularExpression("COD|Banking")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "nvarchar(20)")]
    [RegularExpression("Pending|Completed|Failed")]
    public string Status { get; set; } = string.Empty;

    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }
}
