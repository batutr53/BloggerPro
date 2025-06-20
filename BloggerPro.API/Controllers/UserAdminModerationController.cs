using BloggerPro.Application.DTOs.Admin;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserAdminModerationController : ControllerBase
{
    private readonly IUserAdminModerationService _userService;

    public UserAdminModerationController(IUserAdminModerationService userService)
    {
        _userService = userService;
    }

    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsersWithRoles()
    {
        var result = await _userService.GetAllUsersWithRolesAsync();
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpPost("update-roles")]
    public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesDto dto)
    {
        var result = await _userService.UpdateUserRolesAsync(dto);
        return StatusCode(result.HttpStatusCode, result);
    }

    [HttpPost("toggle-block")]
    public async Task<IActionResult> ToggleUserBlock([FromBody] ToggleUserBlockDto dto)
    {
        var result = await _userService.ToggleUserBlockAsync(dto);
        return StatusCode(result.HttpStatusCode, result);
    }
}
