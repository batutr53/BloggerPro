using BloggerPro.Application.DTOs.TeamMember;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface ITeamMemberService
{
    Task<DataResult<List<TeamMemberListDto>>> GetAllTeamMembersAsync();
    Task<DataResult<TeamMemberListDto>> GetTeamMemberByIdAsync(Guid id);
    Task<Result> CreateTeamMemberAsync(TeamMemberCreateDto teamMemberCreateDto);
    Task<Result> UpdateTeamMemberAsync(TeamMemberUpdateDto teamMemberUpdateDto);
    Task<Result> DeleteTeamMemberAsync(Guid id);
    Task<Result> ToggleTeamMemberStatusAsync(Guid id);
}