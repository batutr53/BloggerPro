using System;
using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class Conversation : BaseEntity
    {
        public Guid User1Id { get; set; }
        public virtual User User1 { get; set; } = null!;
        
        public Guid User2Id { get; set; }
        public virtual User User2 { get; set; } = null!;
        
        public Guid? LastMessageId { get; set; }
        public virtual Message? LastMessage { get; set; }
        
        public new DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
    }
}