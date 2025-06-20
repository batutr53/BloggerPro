using BloggerPro.Application.DTOs.Auth;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloggerPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Kullanıcı Kayıt
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return StatusCode(result.HttpStatusCode, result);
    }

    /// <summary>
    /// Kullanıcı Giriş
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return StatusCode(result.HttpStatusCode, result);
    }

    /// <summary>
    /// Kendi Bilgilerini Görüntüle
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(ClaimTypes.Name)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new ErrorResult("Kullanıcı bulunamadı", 401));

        var result = await _authService.GetMeAsync(userId);
        return StatusCode(result.HttpStatusCode, result);
    }
}
