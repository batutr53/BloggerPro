using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>();
    }

}
