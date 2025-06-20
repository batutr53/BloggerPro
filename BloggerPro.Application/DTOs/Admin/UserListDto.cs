namespace BloggerPro.Application.DTOs.Admin
{
    public class UserListDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
