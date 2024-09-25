using System;
using System.Collections.Generic;

namespace BusssinessObject;

public partial class Message
{
    public int MessageId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string? Subject { get; set; }

    public string? MessageBody { get; set; }

    public DateTime? SentDate { get; set; }

    public bool? IsRead { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User? Sender { get; set; }
}
