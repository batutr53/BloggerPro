using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities;

public class Footer : BaseEntity
{
    public string SectionTitle { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string FooterType { get; set; } = string.Empty; // "section", "link", "newsletter", "copyright"
    public string? IconClass { get; set; }
    public string? ParentSection { get; set; }
}