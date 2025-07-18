using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Application.DTOs.UserDashboard;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserDashboardController : ControllerBase
    {
        private readonly IUserDashboardService _userDashboardService;
        private readonly ILogger<UserDashboardController> _logger;

        public UserDashboardController(IUserDashboardService userDashboardService, ILogger<UserDashboardController> logger)
        {
            _userDashboardService = userDashboardService;
            _logger = logger;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetUserDashboardStats()
        {
            var userId = GetUserId();
            var result = await _userDashboardService.GetUserDashboardStatsAsync(userId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("activities")]
        public async Task<IActionResult> GetUserActivities([FromQuery] int take = 10)
        {
            var userId = GetUserId();
            var result = await _userDashboardService.GetUserActivitiesAsync(userId, take);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("recent-posts")]
        public async Task<IActionResult> GetRecentPosts([FromQuery] int take = 10)
        {
            var userId = GetUserId();
            var result = await _userDashboardService.GetRecentPostsAsync(userId, take);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("active-sessions")]
        public async Task<IActionResult> GetActiveReadingSessions()
        {
            var userId = GetUserId();
            var result = await _userDashboardService.GetActiveReadingSessionsAsync(userId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("reading-stats")]
        public async Task<IActionResult> GetReadingStats()
        {
            var userId = GetUserId();
            var result = await _userDashboardService.GetReadingStatsAsync(userId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetMonthlyReadingStats()
        {
            var userId = GetUserId();
            var result = await _userDashboardService.GetMonthlyReadingStatsAsync(userId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("favorite-categories")]
        public async Task<IActionResult> GetFavoriteCategories([FromQuery] int take = 5)
        {
            var userId = GetUserId();
            var result = await _userDashboardService.GetFavoriteCategoriesAsync(userId, take);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("track-view")]
        public async Task<IActionResult> TrackPostView([FromBody] TrackViewRequest request)
        {
            var userId = GetUserId();
            var ipAddress = GetIpAddress();
            var userAgent = GetUserAgent();
            
            var result = await _userDashboardService.TrackPostViewAsync(userId, request.PostId, ipAddress, userAgent);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("start-session")]
        public async Task<IActionResult> StartReadingSession([FromBody] StartReadingSessionRequest request)
        {
            var userId = GetUserId();
            var ipAddress = GetIpAddress();
            var userAgent = GetUserAgent();
            
            var result = await _userDashboardService.StartReadingSessionAsync(
                userId, request.PostId, ipAddress, userAgent, request.DeviceType, request.ReferrerUrl);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("update-session")]
        public async Task<IActionResult> UpdateReadingSession([FromBody] UpdateReadingSessionRequest request)
        {
            var userId = GetUserId();
            var result = await _userDashboardService.UpdateReadingSessionAsync(
                userId, request.PostId, request.ReadingTimeSeconds, request.ScrollPercentage);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("complete-session")]
        public async Task<IActionResult> CompleteReadingSession([FromBody] CompleteReadingSessionRequest request)
        {
            var userId = GetUserId();
            var result = await _userDashboardService.CompleteReadingSessionAsync(userId, request.PostId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string GetIpAddress()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }
            
            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }
            
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private string GetUserAgent()
        {
            return Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        }
    }

    public class TrackViewRequest
    {
        public Guid PostId { get; set; }
    }

    public class StartReadingSessionRequest
    {
        public Guid PostId { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string ReferrerUrl { get; set; } = string.Empty;
    }

    public class UpdateReadingSessionRequest
    {
        public Guid PostId { get; set; }
        public int ReadingTimeSeconds { get; set; }
        public int ScrollPercentage { get; set; }
    }

    public class CompleteReadingSessionRequest
    {
        public Guid PostId { get; set; }
    }
}