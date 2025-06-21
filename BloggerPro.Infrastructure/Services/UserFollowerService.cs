using AutoMapper;
using BloggerPro.Application.DTOs.Notification;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class UserFollowerService : IUserFollowerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public UserFollowerService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
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
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = followingId,
                Message = $"{follower.UserName} sizi takip etmeye başladı.",
                Type = "NewFollower",
                RelatedEntityId = followerId,
                RelatedEntityType = "User"
            });

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

    }
}
