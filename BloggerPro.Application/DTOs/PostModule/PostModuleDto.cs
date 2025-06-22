using BloggerPro.Application.DTOs.SeoMetadata;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.PostModule
{
    public class PostModuleDto
    {
        public Guid Id { get; set; }
        public PostModuleType Type { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public string? Alignment { get; set; }
        public string? Width { get; set; }
        public int Order { get; set; }
        public int SortOrder { get; set; }
        public List<SeoMetadataDto> SeoMetadata { get; set; } = new();

    }

}
