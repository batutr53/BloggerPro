using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Application.Interfaces;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Constants;
using BloggerPro.Domain.Enums;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICurrentUserService _currentUserService;

        public PostsController(IPostService postService, ICurrentUserService currentUserService)
        {
            _postService = postService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] PostFilterDto filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _postService.GetAllPostsAsync(filter, page, pageSize);
            return HandleResult(result);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedPosts([FromQuery] int count = 5)
        {
            var result = await _postService.GetFeaturedPostsAsync(count);
            return HandleResult(result);
        }
 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            var userId = _currentUserService.UserId;
            var result = await _postService.GetPostByIdAsync(id, userId);
            return HandleResult(result);
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Editor)]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto dto)
        {
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var result = await _postService.CreatePostAsync(dto, _currentUserService.UserId.Value);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] PostUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var result = await _postService.UpdatePostAsync(dto, _currentUserService.UserId.Value);
            return HandleResult(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var result = await _postService.DeletePostAsync(id, _currentUserService.UserId.Value);
            return HandleResult(result);
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Editor)]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePostStatus(Guid id, [FromBody] UpdatePostStatusDto dto)
        {
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var result = await _postService.UpdatePostStatusAsync(id, dto.Status, _currentUserService.UserId.Value);
            return HandleResult(result);
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Editor)]
        [HttpPut("{id}/visibility")]
        public async Task<IActionResult> UpdatePostVisibility(Guid id, [FromBody] UpdatePostVisibilityDto dto)
        {
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var result = await _postService.UpdatePostVisibilityAsync(id, dto.Visibility, _currentUserService.UserId.Value);
            return HandleResult(result);
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Editor)]
        [HttpPut("{id}/reorder-modules")]
        public async Task<IActionResult> ReorderModules(Guid id, [FromBody] List<ModuleSortOrderDto> newOrder)
        {
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var result = await _postService.ReorderModulesAsync(id, newOrder, _currentUserService.UserId.Value);
            return HandleResult(result);
        }

        private IActionResult HandleResult(Result result)
        {
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        private IActionResult HandleResult<T>(DataResult<T> result)
        {
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }

    public class UpdatePostStatusDto
    {
        public PostStatus Status { get; set; }
    }

    public class UpdatePostVisibilityDto
    {
        public PostVisibility Visibility { get; set; }
    }
}
