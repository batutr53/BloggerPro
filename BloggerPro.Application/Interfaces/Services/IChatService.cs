using BloggerPro.Application.DTOs.Chat;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<DataResult<List<ConversationDto>>> GetUserConversationsAsync(Guid userId);
        Task<DataResult<List<MessageDto>>> GetConversationMessagesAsync(Guid userId, Guid otherUserId, int page = 1, int pageSize = 50);
        Task<DataResult<MessageDto>> SendMessageAsync(Guid senderId, CreateMessageDto messageDto);
        Task<Result> MarkMessagesAsReadAsync(Guid userId, MarkMessagesAsReadDto markAsReadDto);
        Task<Result> MarkMessagesAsDeliveredAsync(List<Guid> messageIds);
        Task<DataResult<bool>> CanUsersStartConversationAsync(Guid userId1, Guid userId2);
    }
}