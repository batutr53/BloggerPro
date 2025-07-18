using BloggerPro.Application.DTOs.Bookmark;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ICurrentUserService _currentUserService;

        public BookmarkController(IBookmarkService bookmarkService, ICurrentUserService currentUserService)
        {
            _bookmarkService = bookmarkService;
            _currentUserService = currentUserService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddBookmark([FromBody] BookmarkCreateDto bookmarkCreateDto)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized();
            }

            bookmarkCreateDto.UserId = userId.Value;
            var result = await _bookmarkService.BookmarkPostAsync(bookmarkCreateDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("remove/{postId}")]
        public async Task<IActionResult> RemoveBookmark(Guid postId)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _bookmarkService.RemoveBookmarkAsync(userId.Value, postId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("is-bookmarked/{postId}")]
        public async Task<IActionResult> IsBookmarked(Guid postId)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _bookmarkService.IsBookmarkedAsync(userId.Value, postId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("my-bookmarks")]
        public async Task<IActionResult> GetMyBookmarks()
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _bookmarkService.GetUserBookmarksAsync(userId.Value);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetBookmarkCount()
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _bookmarkService.GetUserBookmarkCountAsync(userId.Value);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("get/{postId}")]
        public async Task<IActionResult> GetBookmark(Guid postId)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _bookmarkService.GetBookmarkAsync(userId.Value, postId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}