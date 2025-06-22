using BloggerPro.Application.DTOs.PostModule;

namespace BloggerPro.Application.DTOs.Post;

public class PostUpdateDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool AllowComments { get; set; }
    public bool IsFeatured { get; set; }
    public int Status { get; set; }
    public int Visibility { get; set; }
    public DateTime? PublishDate { get; set; }

    public List<Guid>? TagIds { get; set; }
    public List<Guid>? CategoryIds { get; set; }

    public List<PostModuleCreateDto>? Modules { get; set; }
}
