using System;

namespace BloggerPro.Application.DTOs.User
{
    public class UserSearchDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string DisplayName => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) 
            ? $"{FirstName} {LastName}" 
            : UserName;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int MutualConnections { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsFollowedBy { get; set; }
        public bool IsMutual => IsFollowing && IsFollowedBy;
    }
}