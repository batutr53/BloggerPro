using BloggerPro.Application.DTOs.Chat;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IPresenceService
    {
        Task<Result> UpdateUserPresenceAsync(Guid userId, bool isOnline, string? connectionId = null);
        Task<DataResult<UserPresenceDto>> GetUserPresenceAsync(Guid userId);
        Task<DataResult<List<UserPresenceDto>>> GetMultipleUserPresenceAsync(List<Guid> userIds);
        Task<Result> UserConnectedAsync(Guid userId, string connectionId);
        Task<Result> UserDisconnectedAsync(Guid userId, string connectionId);
    }
}