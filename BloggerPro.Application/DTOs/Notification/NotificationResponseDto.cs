using System;

namespace BloggerPro.Application.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public string Type { get; set; } = null!;
        public Guid? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
