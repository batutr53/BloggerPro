namespace BloggerPro.Application.DTOs.Bookmark
{
    public class BookmarkCreateDto
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string? Notes { get; set; }
    }
}