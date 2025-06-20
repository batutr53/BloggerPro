using BloggerPro.Domain.Common;
using System.Xml.Linq;

namespace BloggerPro.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Content { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<PostLike> Likes { get; set; } = new HashSet<PostLike>();
    public ICollection<PostRating> Ratings { get; set; } = new HashSet<PostRating>();
    public ICollection<PostCategory> PostCategories { get; set; } = new HashSet<PostCategory>();
    public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>();
    public ICollection<PostModule> Modules { get; set; } = new HashSet<PostModule>();

}
