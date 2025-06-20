using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public bool IsBlocked { get; set; } = false;

    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public ICollection<CommentLike> CommentLikes { get; set; } = new HashSet<CommentLike>();
    public ICollection<PostLike> PostLikes { get; set; } = new HashSet<PostLike>();
    public ICollection<PostRating> PostRatings { get; set; } = new HashSet<PostRating>();



}
