using BloggerPro.Application.DTOs.Notification;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Http;


namespace BloggerPro.Application.Interfaces.Services
{
    public interface IUserService
    {
        // Profile Management
        Task<DataResult<UserProfileDto>> GetUserProfileAsync(Guid userId, Guid? currentUserId = null);
        Task<Result> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto);
        Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task<DataResult<string>> UploadProfileImageAsync(Guid userId, IFormFile file); 

        // Following/Followers
        Task<Result> FollowUserAsync(Guid followerId, Guid followingId);
        Task<Result> UnfollowUserAsync(Guid followerId, Guid followingId);
        Task<DataResult<List<UserFollowerDto>>> GetUserFollowersAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<DataResult<List<UserFollowerDto>>> GetUserFollowingAsync(Guid userId, int page = 1, int pageSize = 20);

        // Notifications
        Task<DataResult<List<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool onlyUnread = false, int page = 1, int pageSize = 20);
        Task<Result> MarkNotificationAsReadAsync(Guid userId, Guid notificationId);
        Task<Result> MarkAllNotificationsAsReadAsync(Guid userId);
    }
}