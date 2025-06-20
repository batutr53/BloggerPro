namespace BloggerPro.Application.DTOs.PostModule
{
    public class PostWithModulesDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public List<Guid> TagIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }

        public List<PostModuleDto> Modules { get; set; } = new();
    }

}
