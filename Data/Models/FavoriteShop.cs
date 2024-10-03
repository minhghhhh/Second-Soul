using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public partial class FavoriteShop
{
    [Key, Column(Order = 0)]
    public int UserId { get; set; }

    [Key, Column(Order = 1)]
    public int ShopId { get; set; }

    public DateTime AddedDate { get; set; } = DateTime.Now;

    [ForeignKey("UserId")]
    public virtual User Shop { get; set; } = null!;

    [ForeignKey("ShopId")]
    public virtual User User { get; set; } = null!;
}
