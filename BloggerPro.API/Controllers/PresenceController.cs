using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]
    public class PresenceController : ControllerBase
    {
        private readonly IPresenceService _presenceService;
        private readonly ILogger<PresenceController> _logger;

        public PresenceController(
            IPresenceService presenceService,
            ILogger<PresenceController> logger)
        {
            _presenceService = presenceService;
            _logger = logger;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdatePresence([FromBody] UpdatePresenceDto updateDto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _presenceService.UpdateUserPresenceAsync(userId, updateDto.IsOnline);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPresence(Guid userId)
        {
            var result = await _presenceService.GetUserPresenceAsync(userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> GetMultipleUserPresence([FromBody] List<Guid> userIds)
        {
            var result = await _presenceService.GetMultipleUserPresenceAsync(userIds);
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

    public class UpdatePresenceDto
    {
        public bool IsOnline { get; set; }
    }
}