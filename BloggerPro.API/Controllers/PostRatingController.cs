using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostRatingController : ControllerBase
{
    private readonly IPostRatingService _postRatingService;

    public PostRatingController(IPostRatingService postRatingService)
    {
        _postRatingService = postRatingService;
    }

    [HttpPost("rate")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> RatePost(Guid postId, int ratingValue)
    {
        if (ratingValue < 1 || ratingValue > 5)
            return BadRequest(new ErrorResult("Puan 1 ile 5 arasında olmalıdır."));

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _postRatingService.RatePostAsync(userId, postId, ratingValue);
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpGet("average/{postId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAverage(Guid postId)
    {
        var result = await _postRatingService.GetAverageRatingAsync(postId);
        return StatusCode(result.HttpStatusCode, result);
    }
    [HttpGet("my-rating/{postId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetMyRating(Guid postId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _postRatingService.GetUserRatingAsync(postId, userId);
        return StatusCode(result.HttpStatusCode, result);
    }
}
