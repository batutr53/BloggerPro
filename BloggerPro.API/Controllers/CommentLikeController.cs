using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CommentLikeController : ControllerBase
    {
        private readonly ICommentLikeService _commentLikeService;

        public CommentLikeController(ICommentLikeService commentLikeService)
        {
            _commentLikeService = commentLikeService;
        }

        [HttpPost("{commentId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Like(Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _commentLikeService.LikeCommentAsync(commentId, userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpDelete("{commentId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Unlike(Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _commentLikeService.UnlikeCommentAsync(commentId, userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("{commentId}/count")]
        [AllowAnonymous]
        public async Task<IActionResult> LikeCount(Guid commentId)
        {
            var result = await _commentLikeService.GetLikeCountAsync(commentId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("{commentId}/has-liked")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> HasLiked(Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _commentLikeService.HasUserLikedAsync(commentId, userId);
            return StatusCode(result.HttpStatusCode, result);
        }
    }
}
