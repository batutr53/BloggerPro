namespace BloggerPro.Application.DTOs.Comment
{
    public class RecentCommentDto
    {
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public PostInfo Post { get; set; }
    }

    public class PostInfo
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Slug { get; set; }
    }
}