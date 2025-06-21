using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.Post;

public class PostDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Excerpt { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string FeaturedImage { get; set; } = null!;
    public int ViewCount { get; set; }
    public bool AllowComments { get; set; }
    public bool IsFeatured { get; set; }
    public PostStatus Status { get; set; }
    public PostVisibility Visibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? PublishDate { get; set; }
    
    // Author information
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string AuthorAvatar { get; set; } = null!;
    
    // Navigation properties
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<PostModuleDto> Modules { get; set; } = new();
    
    // Stats
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    public double? AverageRating { get; set; }
    
    // Current user specific
    public bool? IsLikedByCurrentUser { get; set; }
    public int? CurrentUserRating { get; set; }
}
