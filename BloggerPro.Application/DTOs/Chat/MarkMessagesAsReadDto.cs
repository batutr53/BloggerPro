using System;
using System.Collections.Generic;

namespace BloggerPro.Application.DTOs.Chat
{
    public class MarkMessagesAsReadDto
    {
        public List<Guid> MessageIds { get; set; } = new List<Guid>();
    }
}