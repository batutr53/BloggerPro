using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class Bookmark : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public DateTime BookmarkedAt { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Post Post { get; set; } = null!;
    }
}