using BloggerPro.Application.DTOs.Post;

namespace BloggerPro.Application.DTOs.Bookmark
{
    public class BookmarkListDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public DateTime BookmarkedAt { get; set; }
        public string? Notes { get; set; }
        public PostListDto Post { get; set; } = null!;
    }
}