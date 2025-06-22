using BloggerPro.Application.DTOs.User;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/user-profile")]
    [Authorize(Roles = "User,Admin")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IUserService userService,
            ILogger<UserProfileController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _userService.GetUserProfileAsync(userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _userService.UpdateProfileAsync(userId, dto);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _userService.ChangePasswordAsync(userId, dto);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _userService.UploadProfileImageAsync(userId, file);
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
