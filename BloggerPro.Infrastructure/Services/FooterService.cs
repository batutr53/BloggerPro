using AutoMapper;
using BloggerPro.Application.DTOs.Footer;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Infrastructure.Services;

public class FooterService : IFooterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FooterService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DataResult<List<FooterListDto>>> GetAllFootersAsync()
    {
        try
        {
            var footers = await _unitOfWork.Footers.GetAllAsync();
            var orderedFooters = footers.OrderBy(f => f.SortOrder).ThenBy(f => f.CreatedAt).ToList();

            var footerDtos = _mapper.Map<List<FooterListDto>>(orderedFooters);
            return new SuccessDataResult<List<FooterListDto>>(footerDtos, "Footer bilgileri başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<List<FooterListDto>>($"Footer bilgileri getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<DataResult<FooterListDto>> GetFooterByIdAsync(Guid id)
    {
        try
        {
            var footer = await _unitOfWork.Footers.GetByIdAsync(id);
            if (footer == null)
            {
                return new ErrorDataResult<FooterListDto>("Footer bulunamadı.");
            }

            var footerDto = _mapper.Map<FooterListDto>(footer);
            return new SuccessDataResult<FooterListDto>(footerDto, "Footer bilgisi başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<FooterListDto>($"Footer bilgisi getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<DataResult<List<FooterListDto>>> GetActiveFootersAsync()
    {
        try
        {
            var allFooters = await _unitOfWork.Footers.GetAllAsync();
            var footers = allFooters.Where(f => f.IsActive).OrderBy(f => f.SortOrder).ThenBy(f => f.CreatedAt).ToList();

            var footerDtos = _mapper.Map<List<FooterListDto>>(footers);
            return new SuccessDataResult<List<FooterListDto>>(footerDtos, "Aktif footer bilgileri başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<List<FooterListDto>>($"Aktif footer bilgileri getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<DataResult<List<FooterListDto>>> GetFootersByTypeAsync(string footerType)
    {
        try
        {
            var allFooters = await _unitOfWork.Footers.GetAllAsync();
            var footers = allFooters.Where(f => f.FooterType == footerType && f.IsActive).OrderBy(f => f.SortOrder).ThenBy(f => f.CreatedAt).ToList();

            var footerDtos = _mapper.Map<List<FooterListDto>>(footers);
            return new SuccessDataResult<List<FooterListDto>>(footerDtos, "Footer bilgileri başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<List<FooterListDto>>($"Footer bilgileri getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> CreateFooterAsync(FooterCreateDto footerCreateDto)
    {
        try
        {
            var footer = _mapper.Map<Footer>(footerCreateDto);
            
            // BaseEntity constructor zaten ayarlıyor ama güvenlik için
            if (footer.Id == Guid.Empty)
                footer.Id = Guid.NewGuid();
            
            if (footer.CreatedAt == DateTime.MinValue)
                footer.CreatedAt = DateTime.UtcNow;
            
            footer.UpdatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Footers.AddAsync(footer);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Footer başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            // Inner exception bilgisini de ekle
            var innerMessage = ex.InnerException?.Message ?? "";
            return new ErrorResult($"Footer oluşturulurken hata oluştu: {ex.Message}. Inner: {innerMessage}");
        }
    }

    public async Task<Result> UpdateFooterAsync(FooterUpdateDto footerUpdateDto)
    {
        try
        {
            var footer = await _unitOfWork.Footers.GetByIdAsync(footerUpdateDto.Id);
            if (footer == null)
            {
                return new ErrorResult("Footer bulunamadı.");
            }

            _mapper.Map(footerUpdateDto, footer);
            
            // UpdatedAt alanını güncelle
            footer.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.Footers.Update(footer);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Footer başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "";
            return new ErrorResult($"Footer güncellenirken hata oluştu: {ex.Message}. Inner: {innerMessage}");
        }
    }

    public async Task<Result> DeleteFooterAsync(Guid id)
    {
        try
        {
            var footer = await _unitOfWork.Footers.GetByIdAsync(id);
            if (footer == null)
            {
                return new ErrorResult("Footer bulunamadı.");
            }

            await _unitOfWork.Footers.DeleteAsync(footer);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Footer başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Footer silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> ToggleFooterStatusAsync(Guid id)
    {
        try
        {
            var footer = await _unitOfWork.Footers.GetByIdAsync(id);
            if (footer == null)
            {
                return new ErrorResult("Footer bulunamadı.");
            }

            footer.IsActive = !footer.IsActive;
            footer.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.Footers.Update(footer);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Footer durumu başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "";
            return new ErrorResult($"Footer durumu güncellenirken hata oluştu: {ex.Message}. Inner: {innerMessage}");
        }
    }
}