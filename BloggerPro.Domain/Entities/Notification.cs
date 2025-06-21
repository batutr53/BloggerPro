using System;
using System.ComponentModel.DataAnnotations;
using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        [Required, MaxLength(500)]
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        
        [Required, MaxLength(50)]
        public string Type { get; set; } = null!;
        
        public Guid? RelatedEntityId { get; set; }
        [MaxLength(50)]
        public string? RelatedEntityType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    }
}