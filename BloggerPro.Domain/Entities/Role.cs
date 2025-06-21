using BloggerPro.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace BloggerPro.Domain.Entities;

public class Role : IdentityRole<Guid>, IEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
