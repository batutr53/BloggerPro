namespace BloggerPro.Application.DTOs.UserDashboard
{
    public class UserDashboardStatsDto
    {
        public int TotalReadPosts { get; set; }
        public int TotalBookmarks { get; set; }
        public int TotalReadingTime { get; set; } // Minutes
        public int TotalActiveDays { get; set; }
        public int PostsReadThisMonth { get; set; }
        public int PostsReadThisWeek { get; set; }
        public double AverageReadingTime { get; set; } // Minutes per post
        public int TotalCommentsLeft { get; set; }
        public int TotalLikesGiven { get; set; }
        public string FavoriteCategory { get; set; } = string.Empty;
        public DateTime LastActivityDate { get; set; }
        public int ConsecutiveActiveDays { get; set; }
    }
}