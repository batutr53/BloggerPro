using BloggerPro.Application.DTOs.TeamMember;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamMemberController : ControllerBase
{
    private readonly ITeamMemberService _teamMemberService;

    public TeamMemberController(ITeamMemberService teamMemberService)
    {
        _teamMemberService = teamMemberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTeamMembers()
    {
        var result = await _teamMemberService.GetAllTeamMembersAsync();

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeamMemberById(Guid id)
    {
        var result = await _teamMemberService.GetTeamMemberByIdAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTeamMember([FromBody] TeamMemberCreateDto teamMemberCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _teamMemberService.CreateTeamMemberAsync(teamMemberCreateDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTeamMember(Guid id, [FromBody] TeamMemberUpdateDto teamMemberUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != teamMemberUpdateDto.Id)
        {
            return BadRequest("URL'deki ID ile DTO'daki ID eşleşmiyor.");
        }

        var result = await _teamMemberService.UpdateTeamMemberAsync(teamMemberUpdateDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTeamMember(Guid id)
    {
        var result = await _teamMemberService.DeleteTeamMemberAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleTeamMemberStatus(Guid id)
    {
        var result = await _teamMemberService.ToggleTeamMemberStatusAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }
}