using BloggerPro.Application.DTOs.AboutUs;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IAboutUsService
{
    Task<DataResult<List<AboutUsListDto>>> GetAllAboutUsAsync();
    Task<DataResult<AboutUsListDto>> GetAboutUsByIdAsync(Guid id);
    Task<Result> CreateAboutUsAsync(AboutUsCreateDto aboutUsCreateDto);
    Task<Result> UpdateAboutUsAsync(AboutUsUpdateDto aboutUsUpdateDto);
    Task<Result> DeleteAboutUsAsync(Guid id);
    Task<Result> ToggleAboutUsStatusAsync(Guid id);
}