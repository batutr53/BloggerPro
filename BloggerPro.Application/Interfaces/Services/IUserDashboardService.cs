using BloggerPro.Application.DTOs.UserDashboard;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IUserDashboardService
    {
        Task<IDataResult<UserDashboardStatsDto>> GetUserDashboardStatsAsync(Guid userId);
        Task<IDataResult<List<UserActivityDto>>> GetUserActivitiesAsync(Guid userId, int take = 10);
        Task<IDataResult<List<RecentPostDto>>> GetRecentPostsAsync(Guid userId, int take = 10);
        Task<IDataResult<List<ReadingSessionDto>>> GetActiveReadingSessionsAsync(Guid userId);
        Task<IResult> TrackPostViewAsync(Guid userId, Guid postId, string ipAddress, string userAgent);
        Task<IResult> UpdateReadingSessionAsync(Guid userId, Guid postId, int readingTimeSeconds, int scrollPercentage);
        Task<IResult> CompleteReadingSessionAsync(Guid userId, Guid postId);
        Task<IDataResult<ReadingSessionDto>> StartReadingSessionAsync(Guid userId, Guid postId, string ipAddress, string userAgent, string deviceType, string referrerUrl = "");
        Task<IDataResult<Dictionary<string, int>>> GetReadingStatsAsync(Guid userId);
        Task<IDataResult<Dictionary<string, int>>> GetMonthlyReadingStatsAsync(Guid userId);
        Task<IDataResult<List<string>>> GetFavoriteCategoriesAsync(Guid userId, int take = 5);
    }
}