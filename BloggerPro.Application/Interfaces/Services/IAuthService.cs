using BloggerPro.Application.DTOs.Auth;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IAuthService
{
    Task<DataResult<TokenDto>> RegisterAsync(RegisterDto dto);
    Task<DataResult<TokenDto>> LoginAsync(LoginDto dto);
    Task<DataResult<UserInfoDto>> GetMeAsync(Guid userId);
}
