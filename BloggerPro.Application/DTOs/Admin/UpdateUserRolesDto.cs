namespace BloggerPro.Application.DTOs.Admin
{
    public class UpdateUserRolesDto
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
