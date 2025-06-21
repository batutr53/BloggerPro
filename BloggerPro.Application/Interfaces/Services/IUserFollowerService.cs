using BloggerPro.Application.DTOs.User;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IUserFollowerService
    {
        Task<Result> FollowUserAsync(Guid followerId, Guid followingId);
        Task<Result> UnfollowUserAsync(Guid followerId, Guid followingId);
        Task<DataResult<List<UserFollowerDto>>> GetUserFollowersAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<DataResult<List<UserFollowerDto>>> GetUserFollowingAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<DataResult<bool>> IsFollowingAsync(Guid followerId, Guid followingId);
    }
}
