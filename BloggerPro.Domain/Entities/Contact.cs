using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities;

public class Contact : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsReplied { get; set; } = false;
    public DateTime? RepliedAt { get; set; }
    public string? AdminReply { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}