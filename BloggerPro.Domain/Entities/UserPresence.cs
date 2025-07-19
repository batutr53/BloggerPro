using System;
using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class UserPresence : IEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        public bool IsOnline { get; set; } = false;
        
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        
        public string? ConnectionId { get; set; }
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}