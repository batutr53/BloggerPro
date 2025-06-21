using System;

namespace BloggerPro.Application.DTOs.User
{
    public class CreateUserFollowerDto
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
    }
}
