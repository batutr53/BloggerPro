namespace BloggerPro.Application.DTOs.Auth;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
