using BloggerPro.Application.DTOs.Tag;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface ITagService
    {
        Task<IDataResult<List<TagListDto>>> GetAllAsync();
        Task<IDataResult<TagListDto>> GetByIdAsync(Guid id);
        Task<IResult> CreateAsync(TagCreateDto dto);
        Task<IResult> UpdateAsync(TagUpdateDto dto);
        Task<IResult> DeleteAsync(Guid id);
    }

}
