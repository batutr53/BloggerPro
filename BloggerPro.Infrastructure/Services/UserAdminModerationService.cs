using AutoMapper;
using BloggerPro.Application.DTOs.Admin;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class UserAdminModerationService : IUserAdminModerationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserAdminModerationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IDataResult<List<UserListDto>>> GetAllUsersWithRolesAsync()
    {
        var users = await _uow.Users.Query()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();

        var dtoList = users.Select(u => new UserListDto
        {
            Id = u.Id,
            UserName = u.Username,
            Email = u.Email,
            IsBlocked = u.IsBlocked,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        }).ToList();

        return new SuccessDataResult<List<UserListDto>>(dtoList);
    }

    public async Task<IResult> UpdateUserRolesAsync(UpdateUserRolesDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(dto.UserId);
        if (user == null)
            return new ErrorResult("Kullanıcı bulunamadı.");

        user.UserRoles.Clear();

        foreach (var roleName in dto.Roles)
        {
            var role = await _uow.Roles.Query().FirstOrDefaultAsync(r => r.Name == roleName);
            if (role != null)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
        }

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        return new SuccessResult("Kullanıcının rolleri güncellendi.");
    }

    public async Task<IResult> ToggleUserBlockAsync(ToggleUserBlockDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(dto.UserId);
        if (user == null)
            return new ErrorResult("Kullanıcı bulunamadı.");

        user.IsBlocked = dto.Block;

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        return new SuccessResult(dto.Block ? "Kullanıcı engellendi." : "Kullanıcının engeli kaldırıldı.");
    }
}
