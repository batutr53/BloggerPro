using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Application.DTOs.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BloggerPro.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IPresenceService _presenceService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IPresenceService presenceService, INotificationService notificationService, ILogger<ChatHub> logger)
        {
            _presenceService = presenceService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId != Guid.Empty)
            {
                await _presenceService.UserConnectedAsync(userId, Context.ConnectionId);
                
                // Join user to their personal group for direct messaging
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                // Notify other users that this user is online
                await Clients.Others.SendAsync("UserConnected", userId);
                
                _logger.LogInformation($"User {userId} connected with connection {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != Guid.Empty)
            {
                await _presenceService.UserDisconnectedAsync(userId, Context.ConnectionId);
                
                // Notify other users that this user is offline
                await Clients.Others.SendAsync("UserDisconnected", userId);
                
                _logger.LogInformation($"User {userId} disconnected from connection {Context.ConnectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Called when a message is sent
        public async Task SendMessage(Guid receiverId, string message)
        {
            var senderId = GetUserId();
            if (senderId == Guid.Empty)
                return;

            // Send message to the receiver's group
            await Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentAt = DateTime.UtcNow
            });

            _logger.LogInformation($"Message sent from {senderId} to {receiverId}");
        }

        // Called when messages are marked as read
        public async Task MarkMessagesAsRead(Guid senderId, List<Guid> messageIds)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return;

            // Notify the sender that their messages have been read
            await Clients.Group($"user_{senderId}").SendAsync("MessagesMarkedAsRead", new
            {
                MessageIds = messageIds,
                ReadBy = userId,
                ReadAt = DateTime.UtcNow
            });

            _logger.LogInformation($"Messages {string.Join(",", messageIds)} marked as read by {userId}");
        }

        // Called when messages are delivered
        public async Task MarkMessagesAsDelivered(List<Guid> messageIds)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return;

            await Clients.Caller.SendAsync("MessagesMarkedAsDelivered", new
            {
                MessageIds = messageIds,
                DeliveredAt = DateTime.UtcNow
            });
        }

        // Called when user starts/stops typing
        public async Task UserTyping(Guid receiverId, bool isTyping)
        {
            var senderId = GetUserId();
            if (senderId == Guid.Empty)
                return;

            await Clients.Group($"user_{receiverId}").SendAsync("UserTyping", new
            {
                UserId = senderId,
                IsTyping = isTyping
            });
        }

        // Send real-time notification
        public async Task SendNotification(NotificationDto notification)
        {
            await Clients.Group($"user_{notification.UserId}").SendAsync("ReceiveNotification", notification);
            _logger.LogInformation($"Notification sent to user {notification.UserId}");
        }

        private Guid GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid or missing user ID in claims");
                return Guid.Empty;
            }
            return userId;
        }
    }
}