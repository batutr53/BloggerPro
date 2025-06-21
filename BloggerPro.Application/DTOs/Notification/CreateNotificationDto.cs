using System;

namespace BloggerPro.Application.DTOs.Notification
{
    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!;
        public Guid? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
    }
}
