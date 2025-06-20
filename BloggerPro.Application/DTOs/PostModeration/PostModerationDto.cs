namespace BloggerPro.Application.DTOs.PostModeration
{
    public class PostModerationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
        public int RatingCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
