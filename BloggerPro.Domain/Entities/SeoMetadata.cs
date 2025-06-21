using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class SeoMetadata : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Keywords { get; set; } = null!;
        public string LanguageCode { get; set; } = null!;
        public Guid? CanonicalGroupId { get; set; }

        // SEO'nun ait olduğu PostModule
        public Guid? PostModuleId { get; set; }
        public PostModule? PostModule { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }

}
