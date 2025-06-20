namespace BloggerPro.Application.DTOs.Post
{
    public class PostFilterDto
    {
        public string? Keyword { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? TagId { get; set; }
        public int? MinRating { get; set; }
        public string? SortBy { get; set; } // "date", "like", "comment"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
