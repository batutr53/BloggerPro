using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class PostRating : IEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
        public int RatingValue { get; set; }
        public DateTime RatedAt { get; set; } = DateTime.UtcNow;

    }

}
