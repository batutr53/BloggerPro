namespace BloggerPro.Application.DTOs.User
{
    public class UserFollowerDto
    {
        public Guid UserId { get; set; }
        public string FollowerId { get; set; }
        public string FollowingId { get; set; }
        public DateTime FollowedAt { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
    }
}
