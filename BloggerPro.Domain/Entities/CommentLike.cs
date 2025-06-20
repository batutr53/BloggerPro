using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities;

public class CommentLike : IEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
}
