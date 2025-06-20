using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostLikeController : ControllerBase
    {
        private readonly IPostLikeService _postLikeService;

        public PostLikeController(IPostLikeService postLikeService)
        {
            _postLikeService = postLikeService;
        }

        [HttpPost("{postId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Like(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _postLikeService.LikePostAsync(postId, userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpDelete("{postId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Unlike(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _postLikeService.UnlikePostAsync(postId, userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("count/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLikeCount(Guid postId)
        {
            var result = await _postLikeService.GetLikeCountAsync(postId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("has-liked/{postId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> HasUserLiked(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _postLikeService.HasUserLikedAsync(postId, userId);
            return StatusCode(result.HttpStatusCode, result);
        }
    }

}
