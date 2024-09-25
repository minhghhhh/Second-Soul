using System;
using System.Collections.Generic;

namespace BusssinessObject;

public partial class FavoriteShop
{
    public int? UserId { get; set; }
    public int? ShopId { get; set; }

    public DateTime? AddedDate { get; set; }

    public virtual User? Shop { get; set; }

    public virtual User? User { get; set; }
}
