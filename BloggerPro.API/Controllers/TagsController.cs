using BloggerPro.Application.DTOs.Tag;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _tagService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await _tagService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagCreateDto dto) =>
        Ok(await _tagService.CreateAsync(dto));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TagUpdateDto dto) =>
        Ok(await _tagService.UpdateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) =>
        Ok(await _tagService.DeleteAsync(id));
}
