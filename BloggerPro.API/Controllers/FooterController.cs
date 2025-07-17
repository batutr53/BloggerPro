using BloggerPro.Application.DTOs.Footer;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FooterController : ControllerBase
{
    private readonly IFooterService _footerService;

    public FooterController(IFooterService footerService)
    {
        _footerService = footerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFooters()
    {
        var result = await _footerService.GetAllFootersAsync();

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveFooters()
    {
        var result = await _footerService.GetActiveFootersAsync();

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("type/{footerType}")]
    public async Task<IActionResult> GetFootersByType(string footerType)
    {
        var result = await _footerService.GetFootersByTypeAsync(footerType);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFooterById(Guid id)
    {
        var result = await _footerService.GetFooterByIdAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFooter([FromBody] FooterCreateDto footerCreateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
            return BadRequest(new { Message = "Model validation failed", Errors = errors });
        }

        var result = await _footerService.CreateFooterAsync(footerCreateDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFooter(Guid id, [FromBody] FooterUpdateDto footerUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != footerUpdateDto.Id)
        {
            return BadRequest("URL'deki ID ile DTO'daki ID eşleşmiyor.");
        }

        var result = await _footerService.UpdateFooterAsync(footerUpdateDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFooter(Guid id)
    {
        var result = await _footerService.DeleteFooterAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleFooterStatus(Guid id)
    {
        var result = await _footerService.ToggleFooterStatusAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }
}