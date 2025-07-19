using System;
using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid SenderId { get; set; }
        public virtual User Sender { get; set; } = null!;
        
        public Guid ReceiverId { get; set; }
        public virtual User Receiver { get; set; } = null!;
        
        public string Content { get; set; } = string.Empty;
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? DeliveredAt { get; set; }
        
        public DateTime? ReadAt { get; set; }
        
        public new bool IsDeleted { get; set; } = false;
    }
}