using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]
    public class UserPanelController : ControllerBase
    {
        private readonly IUserPanelService _userPanelService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserPanelController(IUserPanelService userPanelService, IHttpContextAccessor httpContextAccessor)
        {
            _userPanelService = userPanelService;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid GetUserId() =>
           Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("rated-posts")]
        public async Task<IActionResult> GetRatedPosts()
        {
            var result = await _userPanelService.GetRatedPostsAsync(GetUserId());
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("commented-posts")]
        public async Task<IActionResult> GetCommentedPosts()
        {
            var result = await _userPanelService.GetCommentedPostsAsync(GetUserId());
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("liked-posts")]
        public async Task<IActionResult> GetLikedPosts()
        {
            var result = await _userPanelService.GetLikedPostsAsync(GetUserId());
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("comments-on-my-posts")]
        public async Task<IActionResult> GetCommentsOnMyPosts()
        {
            var result = await _userPanelService.GetCommentsOnMyPostsAsync(GetUserId());
            return StatusCode(result.HttpStatusCode, result);
        }
    }

}
