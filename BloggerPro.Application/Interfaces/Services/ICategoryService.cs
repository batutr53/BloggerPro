using BloggerPro.Application.DTOs.Category;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IDataResult<List<CategoryListDto>>> GetAllAsync();
        Task<IDataResult<CategoryListDto>> GetByIdAsync(Guid id);
        Task<IResult> CreateAsync(CategoryCreateDto dto);
        Task<IResult> UpdateAsync(CategoryUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
    }

}
