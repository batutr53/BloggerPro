using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.Post;

public class PostFilterDto
{
    // Search and filter
    public string? Keyword { get; set; }
    public Guid? AuthorId { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public List<Guid>? TagIds { get; set; }
    public int? MinRating { get; set; }
    public bool? IsFeatured { get; set; }
    public PostStatus? Status { get; set; }
    public PostVisibility? Visibility { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    // Sorting
    public string? SortBy { get; set; } = "date"; // "date", "title", "views", "likes", "comments", "rating"
    public bool SortDescending { get; set; } = true;
    
    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Related data
    public bool IncludeAuthor { get; set; } = true;
    public bool IncludeCategories { get; set; } = true;
    public bool IncludeTags { get; set; } = true;
    public bool IncludeModules { get; set; } = false;
    public bool IncludeStats { get; set; } = true;
    
    // Current user context
    public bool IncludeUserSpecificData { get; set; } = false;
}
