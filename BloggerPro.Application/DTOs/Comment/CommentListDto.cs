namespace BloggerPro.Application.DTOs.Comment
{
    public class CommentListDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string Username { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public List<CommentListDto> Replies { get; set; } = new();
    }
}
