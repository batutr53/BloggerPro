using AutoMapper;
using BloggerPro.Application.DTOs.Notification;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.SignalR;
using BloggerPro.Infrastructure.Hubs;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class UserFollowerService : IUserFollowerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<ChatHub> _hubContext;

        public UserFollowerService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService,
            IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        public async Task<Result> FollowUserAsync(Guid followerId, Guid followingId)
        {
            if (followerId == followingId)
                return new ErrorResult("Kendinizi takip edemezsiniz.");

            var isFollowing = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == followerId && uf.FollowingId == followingId)
                .AnyAsync();

            if (isFollowing)
                return new ErrorResult("Zaten bu kullanıcıyı takip ediyorsunuz.");

            var followerExists = await _unitOfWork.Users.AnyAsync(u => u.Id == followerId);
            var followingExists = await _unitOfWork.Users.AnyAsync(u => u.Id == followingId);

            if (!followerExists || !followingExists)
                return new ErrorResult("Kullanıcı bulunamadı.");

            var userFollower = new UserFollower
            {
                FollowerId = followerId,
                FollowingId = followingId,
                FollowedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserFollowers.AddAsync(userFollower);
            await _unitOfWork.SaveChangesAsync();

            // Create notification
            var follower = await _unitOfWork.Users.GetByIdAsync(followerId);
            var notificationResult = await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = followingId,
                Message = $"{follower.UserName} sizi takip etmeye başladı.",
                Type = "NewFollower",
                RelatedEntityId = followerId,
                RelatedEntityType = "User"
            });

            // Send real-time notification
            if (notificationResult.Success)
            {
                await _hubContext.Clients.Group($"user_{followingId}")
                    .SendAsync("ReceiveNotification", notificationResult.Data);
            }

            return new SuccessResult("Kullanıcı takip edildi.");
        }

        public async Task<Result> UnfollowUserAsync(Guid followerId, Guid followingId)
        {
            var userFollower = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == followerId && uf.FollowingId == followingId)
                .FirstOrDefaultAsync();

            if (userFollower == null)
                return new ErrorResult("Bu kullanıcıyı takip etmiyorsunuz.");

            await _unitOfWork.UserFollowers.DeleteAsync(userFollower);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Kullanıcı takipten çıkarıldı.");
        }

        public async Task<DataResult<List<UserFollowerDto>>> GetUserFollowersAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var followers = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowingId == userId)
                .Include(uf => uf.Follower)
                .OrderByDescending(uf => uf.FollowedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = _mapper.Map<List<UserFollowerDto>>(followers);
            return new SuccessDataResult<List<UserFollowerDto>>(result);
        }

        public async Task<DataResult<List<UserFollowerDto>>> GetUserFollowingAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var following = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId)
                .Include(uf => uf.Following)
                .OrderByDescending(uf => uf.FollowedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = _mapper.Map<List<UserFollowerDto>>(following);
            return new SuccessDataResult<List<UserFollowerDto>>(result);
        }

        public async Task<DataResult<bool>> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            var isFollowing = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == followerId && uf.FollowingId == followingId)
                .AnyAsync();

            return new SuccessDataResult<bool>(isFollowing);
        }

        public async Task<DataResult<List<UserFollowerDto>>> GetMutualFollowersAsync(Guid userId, Guid otherUserId)
        {
            // Get users that both userId and otherUserId follow
            // Get users that current user follows
            var userFollowingIds = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            // Get users that the other user follows
            var otherUserFollowingIds = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == otherUserId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            // Find mutual followings
            var mutualFollowingIds = userFollowingIds.Intersect(otherUserFollowingIds).ToList();

            var mutualFollowing = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId && mutualFollowingIds.Contains(uf.FollowingId))
                .Include(uf => uf.Following)
                .ToListAsync();

            var result = _mapper.Map<List<UserFollowerDto>>(mutualFollowing);
            return new SuccessDataResult<List<UserFollowerDto>>(result);
        }

        public async Task<DataResult<List<UserRecommendationDto>>> GetUserRecommendationsAsync(Guid userId, int limit = 10)
        {
            // Get users that are followed by people the current user follows (2nd degree connections)
            // Get users that are followed by people the current user follows (2nd degree connections)
            var userFollowings = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            var secondDegreeConnections = await _unitOfWork.UserFollowers
                .FindByCondition(uf => userFollowings.Contains(uf.FollowerId) && uf.FollowingId != userId)
                .Include(uf => uf.Following)
                .Include(uf => uf.Follower)
                .ToListAsync();

            // Filter out users already followed by current user
            var currentUserFollowings = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            var recommendations = secondDegreeConnections
                .Where(uf => !currentUserFollowings.Contains(uf.FollowingId))
                .GroupBy(uf => uf.FollowingId)
                .Select(g => new { 
                    User = g.First().Following, 
                    ConnectionCount = g.Count(),
                    FollowedBy = g.Select(x => x.Follower.UserName).ToList()
                })
                .OrderByDescending(x => x.ConnectionCount)
                .Take(limit)
                .ToList();

            var result = recommendations.Select(r => new UserRecommendationDto
            {
                Id = r.User.Id,
                UserName = r.User.UserName,
                FirstName = r.User.FirstName,
                LastName = r.User.LastName,
                Bio = r.User.Bio,
                ProfileImageUrl = r.User.ProfileImageUrl,
                FollowedBy = r.FollowedBy
            }).ToList();

            return new SuccessDataResult<List<UserRecommendationDto>>(result);
        }

        public async Task<DataResult<bool>> AreMutualFollowersAsync(Guid userId1, Guid userId2)
        {
            var user1FollowsUser2 = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId1 && uf.FollowingId == userId2)
                .AnyAsync();

            var user2FollowsUser1 = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId2 && uf.FollowingId == userId1)
                .AnyAsync();

            return new SuccessDataResult<bool>(user1FollowsUser2 && user2FollowsUser1);
        }

    }
}
