using BloggerPro.Application.DTOs.Comment;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Yorum ekler (Sadece giriş yapmış kullanıcı)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("Geçerli kullanıcı bilgisi alınamadı.");

            var result = await _commentService.AddCommentAsync(dto, userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        /// <summary>
        /// Yorumu siler (Sadece yorumu yapan kullanıcı veya Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("Geçerli kullanıcı bilgisi alınamadı.");

            var result = await _commentService.DeleteCommentAsync(id, userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        /// <summary>
        /// Belirli bir post'a ait tüm üst yorumları getirir
        /// </summary>
        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(Guid postId)
        {
            var result = await _commentService.GetCommentsByPostAsync(postId);
            return StatusCode(result.HttpStatusCode, result);
        }

        /// <summary>
        /// Son eklenen yorumları getirir
        /// </summary>
        [HttpGet("recent")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentComments([FromQuery] int count = 5)
        {
            var result = await _commentService.GetRecentCommentsAsync(count);
            return StatusCode(result.HttpStatusCode, result);
        }
    }
}
