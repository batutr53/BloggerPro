using AutoMapper;
using BloggerPro.Application.DTOs.AboutUs;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class AboutUsService : IAboutUsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AboutUsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DataResult<List<AboutUsListDto>>> GetAllAboutUsAsync()
    {
        try
        {
            var aboutUsList = await _unitOfWork.AboutUs.Query()
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.SortOrder)
                .ThenBy(a => a.CreatedAt)
                .ToListAsync();

            var aboutUsListDto = _mapper.Map<List<AboutUsListDto>>(aboutUsList);
            return new SuccessDataResult<List<AboutUsListDto>>(aboutUsListDto);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<List<AboutUsListDto>>($"Hakkımızda bilgileri listelenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<DataResult<AboutUsListDto>> GetAboutUsByIdAsync(Guid id)
    {
        try
        {
            var aboutUs = await _unitOfWork.AboutUs.GetByIdAsync(id);
            if (aboutUs == null || aboutUs.IsDeleted)
            {
                return new ErrorDataResult<AboutUsListDto>("Hakkımızda bilgisi bulunamadı.");
            }

            var aboutUsDto = _mapper.Map<AboutUsListDto>(aboutUs);
            return new SuccessDataResult<AboutUsListDto>(aboutUsDto);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<AboutUsListDto>($"Hakkımızda bilgisi getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> CreateAboutUsAsync(AboutUsCreateDto aboutUsCreateDto)
    {
        try
        {
            var aboutUs = _mapper.Map<AboutUs>(aboutUsCreateDto);
            aboutUs.CreatedAt = DateTime.UtcNow;
            aboutUs.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.AboutUs.AddAsync(aboutUs);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Hakkımızda bilgisi başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Hakkımızda bilgisi oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAboutUsAsync(AboutUsUpdateDto aboutUsUpdateDto)
    {
        try
        {
            var aboutUs = await _unitOfWork.AboutUs.GetByIdAsync(aboutUsUpdateDto.Id);
            if (aboutUs == null || aboutUs.IsDeleted)
            {
                return new ErrorResult("Hakkımızda bilgisi bulunamadı.");
            }

            _mapper.Map(aboutUsUpdateDto, aboutUs);
            aboutUs.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AboutUs.Update(aboutUs);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Hakkımızda bilgisi başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Hakkımızda bilgisi güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAboutUsAsync(Guid id)
    {
        try
        {
            var aboutUs = await _unitOfWork.AboutUs.GetByIdAsync(id);
            if (aboutUs == null || aboutUs.IsDeleted)
            {
                return new ErrorResult("Hakkımızda bilgisi bulunamadı.");
            }

            aboutUs.IsDeleted = true;
            aboutUs.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AboutUs.Update(aboutUs);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Hakkımızda bilgisi başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Hakkımızda bilgisi silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> ToggleAboutUsStatusAsync(Guid id)
    {
        try
        {
            var aboutUs = await _unitOfWork.AboutUs.GetByIdAsync(id);
            if (aboutUs == null || aboutUs.IsDeleted)
            {
                return new ErrorResult("Hakkımızda bilgisi bulunamadı.");
            }

            aboutUs.IsActive = !aboutUs.IsActive;
            aboutUs.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AboutUs.Update(aboutUs);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult($"Hakkımızda bilgisi durumu {(aboutUs.IsActive ? "aktif" : "pasif")} olarak güncellendi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Hakkımızda bilgisi durumu güncellenirken hata oluştu: {ex.Message}");
        }
    }
}