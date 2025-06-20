using BloggerPro.Application.DTOs.Admin;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IUserAdminModerationService
{
    Task<IDataResult<List<UserListDto>>> GetAllUsersWithRolesAsync();
    Task<IResult> UpdateUserRolesAsync(UpdateUserRolesDto dto);
    Task<IResult> ToggleUserBlockAsync(ToggleUserBlockDto dto);
}
