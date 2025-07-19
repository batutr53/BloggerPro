using System;

namespace BloggerPro.Application.DTOs.Chat
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderUserName { get; set; } = string.Empty;
        public Guid ReceiverId { get; set; }
        public string ReceiverUserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead => ReadAt.HasValue;
        public bool IsDelivered => DeliveredAt.HasValue;
    }
}