using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Shared.Utilities.Results;
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
    [HttpPost("{postId}/modules/reorder")]
    public async Task<IActionResult> ReorderModules(Guid postId, [FromBody] List<ModuleSortOrderDto> list) { return StatusCode((await _postService.ReorderModulesAsync(postId, list)).HttpStatusCode); }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostWithModules(Guid id)
    {
        var result = await _postService.GetPostWithModulesAsync(id);
        return StatusCode(result.HttpStatusCode, result);
    }
    [HttpPost("filter")]
    [AllowAnonymous]
    public async Task<IActionResult> FilterPosts([FromBody] PostFilterDto dto)
    {
        var result = await _postService.FilterPostsAsync(dto);
        return Ok(result);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _postService.GetPostsPagedAsync();
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDetail(string slug)
    {
        var result = await _postService.GetPostBySlugAsync(slug);
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Create([FromBody] PostCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var result = await _postService.CreatePostAsync(dto, Guid.Parse(userId));
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Update([FromBody] PostUpdateDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _postService.UpdatePostAsync(dto, userId);
        return StatusCode(result.HttpStatusCode, result);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _postService.DeletePostAsync(id);
        return StatusCode(result.HttpStatusCode, result);
    }
}
