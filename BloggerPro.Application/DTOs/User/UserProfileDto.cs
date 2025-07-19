namespace BloggerPro.Application.DTOs.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public DateTime MemberSince { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? BirthDate { get; set; }

        // Social
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? YouTubeUrl { get; set; }

        // Stats
        public int PostCount { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool? IsFollowing { get; set; }
    }

}
