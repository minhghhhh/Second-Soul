using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusssinessObject;

public partial class Message
{
    [Key]
    public int MessageId { get; set; }

    [Required]
    public int SenderId { get; set; }

    [Required]
    public int ReceiverId { get; set; }

    [MaxLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string MessageBody { get; set; } = string.Empty;

    public DateTime SentDate { get; set; } = default;

    public bool IsRead { get; set; } = false;

    [ForeignKey("ReceiverId")]
    public virtual User Receiver { get; set; } = null!;

    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; } = null!;
}
