using BloggerPro.Application.DTOs.AboutUs;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AboutUsController : ControllerBase
{
    private readonly IAboutUsService _aboutUsService;

    public AboutUsController(IAboutUsService aboutUsService)
    {
        _aboutUsService = aboutUsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAboutUs()
    {
        var result = await _aboutUsService.GetAllAboutUsAsync();

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAboutUsById(Guid id)
    {
        var result = await _aboutUsService.GetAboutUsByIdAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAboutUs([FromBody] AboutUsCreateDto aboutUsCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _aboutUsService.CreateAboutUsAsync(aboutUsCreateDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAboutUs(Guid id, [FromBody] AboutUsUpdateDto aboutUsUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != aboutUsUpdateDto.Id)
        {
            return BadRequest("URL'deki ID ile DTO'daki ID eşleşmiyor.");
        }

        var result = await _aboutUsService.UpdateAboutUsAsync(aboutUsUpdateDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAboutUs(Guid id)
    {
        var result = await _aboutUsService.DeleteAboutUsAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleAboutUsStatus(Guid id)
    {
        var result = await _aboutUsService.ToggleAboutUsStatusAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }
}