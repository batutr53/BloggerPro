using System;
using System.Collections.Generic;

namespace BloggerPro.Application.DTOs.User
{
    public class UserRecommendationDto
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
        public List<string> FollowedBy { get; set; } = new List<string>();
        public int MutualConnectionsCount => FollowedBy.Count;
    }
}