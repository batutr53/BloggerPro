namespace BloggerPro.Application.DTOs.Footer;

public class FooterListDto
{
    public Guid Id { get; set; }
    public string SectionTitle { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string FooterType { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public string? ParentSection { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}