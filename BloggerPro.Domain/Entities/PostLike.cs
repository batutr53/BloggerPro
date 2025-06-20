﻿using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class PostLike : IEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }

}
