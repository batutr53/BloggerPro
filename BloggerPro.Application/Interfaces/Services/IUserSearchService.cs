using BloggerPro.Application.DTOs.User;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IUserSearchService
    {
        Task<DataResult<List<UserSearchDto>>> SearchUsersAsync(string query, Guid? currentUserId = null, bool includeMutualConnections = true, int limit = 20);
        Task<DataResult<List<UserRecommendationDto>>> GetUserRecommendationsAsync(Guid userId, int limit = 10);
    }
}