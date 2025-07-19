using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSearchController : ControllerBase
    {
        private readonly IUserSearchService _userSearchService;
        private readonly IUserFollowerService _userFollowerService;
        private readonly ILogger<UserSearchController> _logger;

        public UserSearchController(
            IUserSearchService userSearchService,
            IUserFollowerService userFollowerService,
            ILogger<UserSearchController> logger)
        {
            _userSearchService = userSearchService;
            _userFollowerService = userFollowerService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string q, 
            [FromQuery] bool mutual = true, 
            [FromQuery] int limit = 20)
        {
            var currentUserId = GetCurrentUserId();
            
            var result = await _userSearchService.SearchUsersAsync(q, currentUserId, mutual, limit);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("recommendations")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int limit = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _userSearchService.GetUserRecommendationsAsync(userId, limit);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("mutuals/{userId}/{otherUserId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetMutualConnections(Guid userId, Guid otherUserId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
                return Unauthorized();

            // Users can only see mutual connections if they're involved in the query
            if (currentUserId != userId && currentUserId != otherUserId)
                return Forbid();

            var result = await _userFollowerService.GetMutualFollowersAsync(userId, otherUserId);
            return StatusCode(result.HttpStatusCode, result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Guid.Empty;
            }
            return userId;
        }
    }
}