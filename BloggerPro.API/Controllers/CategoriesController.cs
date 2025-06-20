using BloggerPro.Application.DTOs.Category;
using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloggerPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto) => Ok(await _service.CreateAsync(dto));

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDto dto) => Ok(await _service.UpdateAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id) => Ok(await _service.DeleteAsync(id));
    }

}
