using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!; // örnek: Admin, User, Editor

    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
}
