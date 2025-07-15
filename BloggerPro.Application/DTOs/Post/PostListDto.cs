using BloggerPro.Application.DTOs.User;

namespace BloggerPro.Application.DTOs.Post;

public class PostListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Excerpt { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string FeaturedImage { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishDate { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public double AverageRating { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public UserDto Author { get; set; } = null!;
    public ICollection<string> Categories { get; set; } = new List<string>();
    public ICollection<string> Tags { get; set; } = new List<string>();
}
