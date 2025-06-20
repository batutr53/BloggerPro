using AutoMapper;
using BloggerPro.Application.DTOs.Auth;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IMapper mapper, IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }


    public async Task<DataResult<TokenDto>> RegisterAsync(RegisterDto dto)
    {
        var exists = await _unitOfWork.Users.Query().AnyAsync(x => x.Email == dto.Email);
        if (exists)
            return new ErrorDataResult<TokenDto>("Bu email zaten kayıtlı.");

        var user = new User
        {
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = "", 
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        var role = await _unitOfWork.Roles.Query().FirstOrDefaultAsync(r => r.Name == "User");
        if (role == null)
            return new ErrorDataResult<TokenDto>("'User' rolü sistemde tanımlı değil.");

        user.UserRoles.Add(new UserRole { RoleId = role.Id, User = user });

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user, new List<string> { role.Name });
        return new SuccessDataResult<TokenDto>(token, "Kayıt başarılı.");
    }

    public async Task<DataResult<TokenDto>> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Users.Query()
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user is null)
            return new ErrorDataResult<TokenDto>("Geçersiz kullanıcı.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return new ErrorDataResult<TokenDto>("Şifre yanlış.");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var token = _jwtService.GenerateToken(user, roles);

        return new SuccessDataResult<TokenDto>(token, "Giriş başarılı.");
    }

    public async Task<DataResult<UserInfoDto>> GetMeAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.Query()
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return new ErrorDataResult<UserInfoDto>("Kullanıcı bulunamadı.");

        var dto = new UserInfoDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return new SuccessDataResult<UserInfoDto>(dto);
    }
}
