using System;

namespace BloggerPro.Application.DTOs.User
{
    public class UserFollowerResponseDto
    {
        public Guid FollowerId { get; set; }
        public string FollowerUsername { get; set; } = null!;
        public string? FollowerProfileImage { get; set; }
        public Guid FollowingId { get; set; }
        public string FollowingUsername { get; set; } = null!;
        public string? FollowingProfileImage { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}
