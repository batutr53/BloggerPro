namespace BloggerPro.Application.DTOs.Admin
{
    public class ToggleUserBlockDto
    {
        public Guid UserId { get; set; }
        public bool Block { get; set; }
    }
}
