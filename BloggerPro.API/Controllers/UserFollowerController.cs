using BloggerPro.Application.DTOs.User;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]
    public class UserFollowerController : ControllerBase
    {
        private readonly IUserFollowerService _userFollowerService;
        private readonly ILogger<UserFollowerController> _logger;

        public UserFollowerController(
            IUserFollowerService userFollowerService,
            ILogger<UserFollowerController> logger)
        {
            _userFollowerService = userFollowerService;
            _logger = logger;
        }

        [HttpPost("follow/{followingId}")]
        public async Task<IActionResult> FollowUser(Guid followingId)
        {
            var followerId = GetCurrentUserId();
            if (followerId == Guid.Empty)
                return Unauthorized();

            var result = await _userFollowerService.FollowUserAsync(followerId, followingId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpDelete("unfollow/{followingId}")]
        public async Task<IActionResult> UnfollowUser(Guid followingId)
        {
            var followerId = GetCurrentUserId();
            if (followerId == Guid.Empty)
                return Unauthorized();

            var result = await _userFollowerService.UnfollowUserAsync(followerId, followingId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("followers/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFollowers(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _userFollowerService.GetUserFollowersAsync(userId, page, pageSize);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("following/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFollowing(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _userFollowerService.GetUserFollowingAsync(userId, page, pageSize);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("is-following/{targetUserId}")]
        public async Task<IActionResult> IsFollowing(Guid targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
                return Unauthorized();

            var result = await _userFollowerService.IsFollowingAsync(currentUserId, targetUserId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("mutuals/{userId}/{otherUserId}")]
        public async Task<IActionResult> GetMutualFollowers(Guid userId, Guid otherUserId)
        {
            var result = await _userFollowerService.GetMutualFollowersAsync(userId, otherUserId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int limit = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _userFollowerService.GetUserRecommendationsAsync(userId, limit);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("are-mutual/{userId1}/{userId2}")]
        public async Task<IActionResult> AreMutualFollowers(Guid userId1, Guid userId2)
        {
            var result = await _userFollowerService.AreMutualFollowersAsync(userId1, userId2);
            return StatusCode(result.HttpStatusCode, result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid or missing user ID in claims");
                return Guid.Empty;
            }
            return userId;
        }
    }
}
