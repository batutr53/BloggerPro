using BloggerPro.Application.DTOs.Contact;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact([FromBody] ContactCreateDto contactCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var result = await _contactService.CreateContactAsync(contactCreateDto, ipAddress, userAgent);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllContacts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? isReplied = null)
    {
        var result = await _contactService.GetAllContactsAsync(page, pageSize, isReplied);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetContactById(Guid id)
    {
        var result = await _contactService.GetContactByIdAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost("{id}/reply")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReplyToContact(Guid id, [FromBody] ContactReplyDto contactReplyDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _contactService.ReplyToContactAsync(id, contactReplyDto);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteContact(Guid id)
    {
        var result = await _contactService.DeleteContactAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPatch("{id}/mark-replied")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAsReplied(Guid id)
    {
        var result = await _contactService.MarkAsRepliedAsync(id);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }
}