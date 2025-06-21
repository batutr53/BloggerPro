using AutoMapper;
using BloggerPro.Application.DTOs.Auth;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(UserManager<User> userManager, RoleManager<Role> roleManager, IJwtService jwtService, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<DataResult<TokenDto>> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return new ErrorDataResult<TokenDto>("Bu email zaten kayıtlı.");

        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Username,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ErrorDataResult<TokenDto>($"Kullanıcı oluşturulamadı: {errors}");
        }

        if (!await _roleManager.RoleExistsAsync("User"))
            return new ErrorDataResult<TokenDto>("'User' rolü sistemde tanımlı değil.");

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles.ToList());

        return new SuccessDataResult<TokenDto>(token, "Kayıt başarılı.");
    }

    public async Task<DataResult<TokenDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return new ErrorDataResult<TokenDto>("Geçersiz kullanıcı.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            return new ErrorDataResult<TokenDto>("Şifre yanlış.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles.ToList());

        return new SuccessDataResult<TokenDto>(token, "Giriş başarılı.");
    }

    public async Task<DataResult<UserInfoDto>> GetMeAsync(Guid userId)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return new ErrorDataResult<UserInfoDto>("Kullanıcı bulunamadı.");

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new UserInfoDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Roles = roles.ToList()
        };

        return new SuccessDataResult<UserInfoDto>(dto);
    }
}
