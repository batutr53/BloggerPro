using BloggerPro.Domain.Entities;
using BloggerPro.Application.DTOs.Auth;

namespace BloggerPro.Application.Interfaces.Services;

public interface IJwtService
{
    TokenDto GenerateToken(User user, List<string> roles);
}
