namespace BloggerPro.Application.DTOs.Auth;

public class TokenDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
