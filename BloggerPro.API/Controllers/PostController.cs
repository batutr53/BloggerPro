using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // Post CRUD
    [Authorize(Roles = $"{UserRoles.User},{UserRoles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostCreateDto dto)
    {
        var result = await _postService.CreatePostAsync(dto, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _postService.GetPostByIdAsync(id);
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _postService.GetPostBySlugAsync(slug);
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpPost("{id:guid}/increment-view")]
    [AllowAnonymous]
    public async Task<IActionResult> IncrementViewCount(Guid id)
    {
        var result = await _postService.IncrementViewCountAsync(id);
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PostUpdateDto dto)
    {
        var result = await _postService.UpdatePostAsync(dto, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _postService.DeletePostAsync(id, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    // Post listing
    [HttpPost("all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromBody] PostFilterDto filter, int page = 1, int pageSize = 10)
    {
        var result = await _postService.GetAllPostsAsync(filter, page, pageSize);
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpPost("GetAllPost")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPost(int page = 1, int pageSize = 10)
    {
        var result = await _postService.GetAllPostsAsync(page, pageSize);
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpPost("my-posts")]
    public async Task<IActionResult> GetByAuthor([FromBody] PostFilterDto filter, int page = 1, int pageSize = 10)
    {
        var result = await _postService.GetPostsByAuthorIdAsync(GetUserId(), filter, page, pageSize);
        return StatusCode(result.HttpStatusCode, result);
    }

    // Post visibility and status
    [Authorize]
    [HttpPut("{postId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid postId, [FromQuery] int status)
    {
        var result = await _postService.UpdatePostStatusAsync(postId, (Domain.Enums.PostStatus)status, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpPut("{postId:guid}/visibility")]
    public async Task<IActionResult> UpdateVisibility(Guid postId, [FromQuery] int visibility)
    {
        var result = await _postService.UpdatePostVisibilityAsync(postId, (Domain.Enums.PostVisibility)visibility, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpPut("{postId:guid}/featured-toggle")]
    public async Task<IActionResult> ToggleFeatured(Guid postId)
    {
        var result = await _postService.TogglePostFeaturedStatusAsync(postId, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    // Modules
    [Authorize]
    [HttpPost("{postId:guid}/modules")]
    public async Task<IActionResult> AddModule(Guid postId, [FromBody] CreatePostModuleDto dto)
    {
        var result = await _postService.AddModuleToPostAsync(postId, dto, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpPut("{postId:guid}/modules")]
    public async Task<IActionResult> UpdateModule(Guid postId, [FromBody] UpdatePostModuleDto dto)
    {
        var result = await _postService.UpdateModuleAsync(postId, dto, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpDelete("{postId:guid}/modules/{moduleId:guid}")]
    public async Task<IActionResult> RemoveModule(Guid postId, Guid moduleId)
    {
        var result = await _postService.RemoveModuleFromPostAsync(postId, moduleId, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpPost("{postId:guid}/modules/reorder")]
    public async Task<IActionResult> ReorderModules(Guid postId, [FromBody] List<ModuleSortOrderDto> newOrder)
    {
        var result = await _postService.ReorderModulesAsync(postId, newOrder, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    // Interactions
    [Authorize]
    [HttpPost("{postId:guid}/like")]
    public async Task<IActionResult> Like(Guid postId)
    {
        var result = await _postService.LikePostAsync(postId, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpDelete("{postId:guid}/like")]
    public async Task<IActionResult> Unlike(Guid postId)
    {
        var result = await _postService.UnlikePostAsync(postId, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpPost("{postId:guid}/rate")]
    public async Task<IActionResult> Rate(Guid postId, [FromQuery] int score)
    {
        var result = await _postService.RatePostAsync(postId, score, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpDelete("{postId:guid}/rate")]
    public async Task<IActionResult> RemoveRating(Guid postId)
    {
        var result = await _postService.RemoveRatingAsync(postId, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    // Stats
    [Authorize]
    [HttpGet("{postId:guid}/stats")]
    public async Task<IActionResult> GetPostStats(Guid postId)
    {
        var result = await _postService.GetPostStatsAsync(postId, GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [Authorize]
    [HttpGet("my-stats")]
    public async Task<IActionResult> GetUserStats()
    {
        var result = await _postService.GetUserPostStatsAsync(GetUserId());
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchPosts([FromQuery] string keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest(new { success = false, message = "Arama kelimesi gerekli." });
        }

        var filter = new PostFilterDto
        {
            Keyword = keyword,
            Status = Domain.Enums.PostStatus.Published
        };

        var result = await _postService.GetAllPostsAsync(filter, page, pageSize);
        return StatusCode(result.HttpStatusCode, result);
    }
}
