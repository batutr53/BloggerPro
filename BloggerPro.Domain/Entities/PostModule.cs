using BloggerPro.Domain.Common;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Domain.Entities;

public class PostModule : BaseEntity
{
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;

    public int SortOrder { get; set; } // Sıralama

    public PostModuleType Type { get; set; }
    public int Order { get; set; }

    public string? Content { get; set; } // Text / Quote için içerik
    public string? MediaUrl { get; set; } // Görsel, video, embed için URL

    public string? Alignment { get; set; } // "left", "right", "center"
    public string? Width { get; set; } // responsive width için: "50%", "100%" vs.
}
