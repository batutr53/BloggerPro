using System;
using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class UserFollower : IEntity
    {
        public Guid FollowerId { get; set; }
        public virtual User Follower { get; set; } = null!;
        public Guid FollowingId { get; set; }
        public virtual User Following { get; set; } = null!;
        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}