using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public ICollection<PostCategory> PostCategories { get; set; } = new HashSet<PostCategory>();
    }

}
