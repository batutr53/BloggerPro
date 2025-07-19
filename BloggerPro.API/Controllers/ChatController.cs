using BloggerPro.Application.DTOs.Chat;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BloggerPro.Infrastructure.Hubs;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IChatService chatService,
            IHubContext<ChatHub> hubContext,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _chatService.GetUserConversationsAsync(userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("messages/{otherUserId}")]
        public async Task<IActionResult> GetMessages(Guid otherUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _chatService.GetConversationMessagesAsync(userId, otherUserId, page, pageSize);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageDto messageDto)
        {
            var senderId = GetCurrentUserId();
            if (senderId == Guid.Empty)
                return Unauthorized();

            var result = await _chatService.SendMessageAsync(senderId, messageDto);
            
            if (result.Success)
            {
                // Send real-time notification via SignalR
                await _hubContext.Clients.Group($"user_{messageDto.ReceiverId}")
                    .SendAsync("ReceiveMessage", result.Data);
            }

            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkMessagesAsReadDto markAsReadDto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _chatService.MarkMessagesAsReadAsync(userId, markAsReadDto);
            
            if (result.Success && markAsReadDto.MessageIds.Any())
            {
                // Get the first message to find the sender
                var messagesResult = await _chatService.GetConversationMessagesAsync(userId, Guid.Empty);
                if (messagesResult.Success)
                {
                    var firstMessage = messagesResult.Data?.FirstOrDefault(m => markAsReadDto.MessageIds.Contains(m.Id));
                    if (firstMessage != null)
                    {
                        // Notify sender that messages were read
                        await _hubContext.Clients.Group($"user_{firstMessage.SenderId}")
                            .SendAsync("MessagesMarkedAsRead", new
                            {
                                MessageIds = markAsReadDto.MessageIds,
                                ReadBy = userId,
                                ReadAt = DateTime.UtcNow
                            });
                    }
                }
            }

            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("can-chat/{otherUserId}")]
        public async Task<IActionResult> CanStartConversation(Guid otherUserId)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _chatService.CanUsersStartConversationAsync(userId, otherUserId);
            return StatusCode(result.HttpStatusCode, result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid or missing user ID in claims");
                return Guid.Empty;
            }
            return userId;
        }
    }
}