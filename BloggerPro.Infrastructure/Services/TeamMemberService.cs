using AutoMapper;
using BloggerPro.Application.DTOs.TeamMember;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class TeamMemberService : ITeamMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeamMemberService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DataResult<List<TeamMemberListDto>>> GetAllTeamMembersAsync()
    {
        try
        {
            var teamMembers = await _unitOfWork.TeamMembers.Query()
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync();

            var teamMembersDto = _mapper.Map<List<TeamMemberListDto>>(teamMembers);
            return new SuccessDataResult<List<TeamMemberListDto>>(teamMembersDto);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<List<TeamMemberListDto>>($"Ekip üyeleri listelenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<DataResult<TeamMemberListDto>> GetTeamMemberByIdAsync(Guid id)
    {
        try
        {
            var teamMember = await _unitOfWork.TeamMembers.GetByIdAsync(id);
            if (teamMember == null || teamMember.IsDeleted)
            {
                return new ErrorDataResult<TeamMemberListDto>("Ekip üyesi bulunamadı.");
            }

            var teamMemberDto = _mapper.Map<TeamMemberListDto>(teamMember);
            return new SuccessDataResult<TeamMemberListDto>(teamMemberDto);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<TeamMemberListDto>($"Ekip üyesi getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> CreateTeamMemberAsync(TeamMemberCreateDto teamMemberCreateDto)
    {
        try
        {
            var teamMember = _mapper.Map<TeamMember>(teamMemberCreateDto);
            teamMember.CreatedAt = DateTime.UtcNow;
            teamMember.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TeamMembers.AddAsync(teamMember);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Ekip üyesi başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ekip üyesi oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> UpdateTeamMemberAsync(TeamMemberUpdateDto teamMemberUpdateDto)
    {
        try
        {
            var teamMember = await _unitOfWork.TeamMembers.GetByIdAsync(teamMemberUpdateDto.Id);
            if (teamMember == null || teamMember.IsDeleted)
            {
                return new ErrorResult("Ekip üyesi bulunamadı.");
            }

            _mapper.Map(teamMemberUpdateDto, teamMember);
            teamMember.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TeamMembers.Update(teamMember);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Ekip üyesi başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ekip üyesi güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> DeleteTeamMemberAsync(Guid id)
    {
        try
        {
            var teamMember = await _unitOfWork.TeamMembers.GetByIdAsync(id);
            if (teamMember == null || teamMember.IsDeleted)
            {
                return new ErrorResult("Ekip üyesi bulunamadı.");
            }

            teamMember.IsDeleted = true;
            teamMember.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TeamMembers.Update(teamMember);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Ekip üyesi başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ekip üyesi silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> ToggleTeamMemberStatusAsync(Guid id)
    {
        try
        {
            var teamMember = await _unitOfWork.TeamMembers.GetByIdAsync(id);
            if (teamMember == null || teamMember.IsDeleted)
            {
                return new ErrorResult("Ekip üyesi bulunamadı.");
            }

            teamMember.IsActive = !teamMember.IsActive;
            teamMember.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TeamMembers.Update(teamMember);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult($"Ekip üyesi durumu {(teamMember.IsActive ? "aktif" : "pasif")} olarak güncellendi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ekip üyesi durumu güncellenirken hata oluştu: {ex.Message}");
        }
    }
}