using BloggerPro.Application.DTOs.Post;

namespace BloggerPro.Application.DTOs.Bookmark
{
    public class BookmarkDetailDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public DateTime BookmarkedAt { get; set; }
        public string? Notes { get; set; }
        public PostDetailDto Post { get; set; } = null!;
    }
}