namespace BloggerPro.Application.DTOs.UserDashboard
{
    public class RecentPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string FeaturedImage { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime VisitedAt { get; set; }
        public int ReadingTimeMinutes { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsLiked { get; set; }
        public int TotalReadingTime { get; set; } // Total time spent on this post
        public int VisitCount { get; set; } // How many times visited
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}