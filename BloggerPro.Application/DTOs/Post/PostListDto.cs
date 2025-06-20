namespace BloggerPro.Application.DTOs.Post;

public class PostListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }  // eklendi
    public int CommentCount { get; set; }  // eklendi
}

