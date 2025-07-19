using AutoMapper;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class UserSearchService : IUserSearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserFollowerService _userFollowerService;

        public UserSearchService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserFollowerService userFollowerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userFollowerService = userFollowerService;
        }

        public async Task<DataResult<List<UserSearchDto>>> SearchUsersAsync(string query, Guid? currentUserId = null, bool includeMutualConnections = true, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new SuccessDataResult<List<UserSearchDto>>(new List<UserSearchDto>());
            }

            query = query.Trim().ToLower();

            var usersQuery = _unitOfWork.Users
                .FindByCondition(u => 
                    u.UserName.ToLower().Contains(query) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(query)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(query)) ||
                    (u.Bio != null && u.Bio.ToLower().Contains(query)));

            // Exclude current user from results
            if (currentUserId.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.Id != currentUserId.Value);
            }

            var users = await usersQuery
                .Take(limit)
                .ToListAsync();

            var userSearchDtos = new List<UserSearchDto>();

            foreach (var user in users)
            {
                var userDto = new UserSearchDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Bio = user.Bio,
                    ProfileImageUrl = user.ProfileImageUrl
                };

                // Calculate mutual connections and follow status if current user is provided
                if (currentUserId.HasValue)
                {
                    // Check if current user follows this user
                    var isFollowing = await _userFollowerService.IsFollowingAsync(currentUserId.Value, user.Id);
                    userDto.IsFollowing = isFollowing.Data;

                    // Check if this user follows current user
                    var isFollowedBy = await _userFollowerService.IsFollowingAsync(user.Id, currentUserId.Value);
                    userDto.IsFollowedBy = isFollowedBy.Data;

                    // Get mutual connections count
                    if (includeMutualConnections)
                    {
                        var mutuals = await _userFollowerService.GetMutualFollowersAsync(currentUserId.Value, user.Id);
                        userDto.MutualConnections = mutuals.Data?.Count ?? 0;
                    }
                }

                userSearchDtos.Add(userDto);
            }

            // Sort by mutual connections if enabled, then by relevance
            if (includeMutualConnections && currentUserId.HasValue)
            {
                userSearchDtos = userSearchDtos
                    .OrderByDescending(u => u.MutualConnections)
                    .ThenByDescending(u => u.IsMutual)
                    .ThenBy(u => u.UserName)
                    .ToList();
            }

            return new SuccessDataResult<List<UserSearchDto>>(userSearchDtos);
        }

        public async Task<DataResult<List<UserRecommendationDto>>> GetUserRecommendationsAsync(Guid userId, int limit = 10)
        {
            return await _userFollowerService.GetUserRecommendationsAsync(userId, limit);
        }
    }
}