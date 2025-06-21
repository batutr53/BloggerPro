namespace BloggerPro.Application.DTOs.SeoMetadata
{
    public class SeoMetadataCreateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CanonicalUrl { get; set; } = null!;
        public string LanguageCode { get; set; } = null!;
        public string Keywords { get; set; } = null!;
        public Guid CanonicalGroupId { get; set; }
    }

}
