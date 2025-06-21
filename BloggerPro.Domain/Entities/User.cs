using BloggerPro.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace BloggerPro.Domain.Entities;

public class User : IdentityUser<Guid>, IEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsBlocked { get; set; } = false;
    public string? Location { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Website { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Social
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? LinkedInUrl { get; set; }

    // Navigation
    public ICollection<CommentLike> CommentLikes { get; set; } = new HashSet<CommentLike>();
    public ICollection<PostLike> PostLikes { get; set; } = new HashSet<PostLike>();
    public ICollection<PostRating> PostRatings { get; set; } = new HashSet<PostRating>();
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    public ICollection<UserFollower> Followers { get; set; } = new HashSet<UserFollower>();
    public ICollection<UserFollower> Following { get; set; } = new HashSet<UserFollower>();
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
}
