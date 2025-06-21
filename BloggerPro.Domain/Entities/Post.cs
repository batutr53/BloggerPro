using BloggerPro.Domain.Common;
using BloggerPro.Domain.Constants;
using BloggerPro.Domain.Enums;
using System.Xml.Linq;

namespace BloggerPro.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Excerpt { get; set; } = null!;
    public string FeaturedImage { get; set; } = null!;
    public DateTime? PublishDate { get; set; }
    public DateTime? LastModified { get; set; }
    public int ViewCount { get; set; }
    public bool IsActive { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public bool AllowComments { get; set; } = true;
    public bool IsFeatured { get; set; }
    public double? AverageRating { get; set; }
    public int RatingCount { get; set; }
    public int LikeCount { get; set; }
    // Relationships
    public Guid AuthorId { get; set; }
    public virtual User Author { get; set; } = null!;
    
    // Navigation properties
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<PostLike> Likes { get; set; } = new HashSet<PostLike>();
    public ICollection<PostRating> Ratings { get; set; } = new HashSet<PostRating>();
    public ICollection<PostCategory> PostCategories { get; set; } = new HashSet<PostCategory>();
    public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>();
    public ICollection<PostModule> Modules { get; set; } = new HashSet<PostModule>();

    // Helper methods
    public bool CanBeViewedBy(User user)
    {
        if (Status != PostStatus.Published)
            return false;

        if (Visibility == PostVisibility.Public)
            return true;

        if (user == null)
            return false;


        if (Visibility == PostVisibility.FollowersOnly)
            return user.Id == AuthorId ||
                   user.Following.Any(f => f.FollowingId == AuthorId);

        return false;
    }
}
