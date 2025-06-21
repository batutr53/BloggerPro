using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.Post
{
    public class UpdatePostStatusDto
    {
        public PostStatus Status { get; set; }
        public DateTime? PublishDate { get; set; }
    }
}
