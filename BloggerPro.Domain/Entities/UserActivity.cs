using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class UserActivity : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public Guid? PostId { get; set; }
        public Post? Post { get; set; }
        
        public string ActivityType { get; set; } = string.Empty; // "PostView", "Bookmark", "Comment", "Like", "Share"
        public string Description { get; set; } = string.Empty;
        
        // Activity specific data (JSON format)
        public string ActivityData { get; set; } = string.Empty;
        
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}