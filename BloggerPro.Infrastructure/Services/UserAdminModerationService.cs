using AutoMapper;
using BloggerPro.Application.DTOs.Admin;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class UserAdminModerationService : IUserAdminModerationService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public UserAdminModerationService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<IDataResult<List<UserListDto>>> GetAllUsersWithRolesAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        var dtoList = new List<UserListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            dtoList.Add(new UserListDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsBlocked = user.IsBlocked,
                Roles = roles.ToList()
            });
        }

        return new SuccessDataResult<List<UserListDto>>(dtoList);
    }

    public async Task<IResult> UpdateUserRolesAsync(UpdateUserRolesDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null)
            return new ErrorResult("Kullanıcı bulunamadı.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return new ErrorResult("Mevcut roller silinemedi.");

        var addResult = await _userManager.AddToRolesAsync(user, dto.Roles);
        if (!addResult.Succeeded)
            return new ErrorResult("Yeni roller eklenemedi.");

        return new SuccessResult("Kullanıcının rolleri güncellendi.");
    }

    public async Task<IResult> ToggleUserBlockAsync(ToggleUserBlockDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null)
            return new ErrorResult("Kullanıcı bulunamadı.");

        user.IsBlocked = dto.Block;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return new ErrorResult("Kullanıcı engelleme durumu güncellenemedi.");

        return new SuccessResult(dto.Block ? "Kullanıcı engellendi." : "Kullanıcının engeli kaldırıldı.");
    }
}
