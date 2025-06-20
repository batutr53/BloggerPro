using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminModerationController : ControllerBase
    {
        private readonly IAdminModerationService _moderationService;

        public AdminModerationController(IAdminModerationService moderationService)
        {
            _moderationService = moderationService;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _moderationService.GetAllPostsAsync();
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPostDetail(Guid id)
        {
            var result = await _moderationService.GetPostDetailsAsync(id);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _moderationService.DeletePostAsync(id);
            return StatusCode(result.HttpStatusCode, result);
        }
    }

}
