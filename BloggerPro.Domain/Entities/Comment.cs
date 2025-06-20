using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;

    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new HashSet<Comment>();

    public ICollection<CommentLike> Likes { get; set; } = new HashSet<CommentLike>();
}
