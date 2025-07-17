using BloggerPro.Application.DTOs.Footer;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IFooterService
{
    Task<DataResult<List<FooterListDto>>> GetAllFootersAsync();
    Task<DataResult<FooterListDto>> GetFooterByIdAsync(Guid id);
    Task<DataResult<List<FooterListDto>>> GetActiveFootersAsync();
    Task<DataResult<List<FooterListDto>>> GetFootersByTypeAsync(string footerType);
    Task<Result> CreateFooterAsync(FooterCreateDto footerCreateDto);
    Task<Result> UpdateFooterAsync(FooterUpdateDto footerUpdateDto);
    Task<Result> DeleteFooterAsync(Guid id);
    Task<Result> ToggleFooterStatusAsync(Guid id);
}