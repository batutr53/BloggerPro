using BloggerPro.Application.DTOs.SeoMetadata;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.PostModule
{
    public class PostModuleUpdateDto
    {
        public Guid Id { get; set; }
        public ModuleType Type { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public int Order { get; set; }
        public Guid? TagId { get; set; }

        public List<SeoMetadataUpdateDto>? SeoMetadata { get; set; }
    }
}
