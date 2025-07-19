using System;

namespace BloggerPro.Application.DTOs.Chat
{
    public class CreateMessageDto
    {
        public Guid ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}