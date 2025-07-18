using BloggerPro.Domain.Common;

namespace BloggerPro.Domain.Entities
{
    public class ReadingSession : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
        
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public int ReadingTimeSeconds { get; set; } = 0;
        public int ScrollPercentage { get; set; } = 0; // 0-100
        public bool IsCompleted { get; set; } = false;
        
        // Session metadata
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty; // Mobile, Desktop, Tablet
        public string ReferrerUrl { get; set; } = string.Empty;
        
        // Reading behavior
        public int PauseCount { get; set; } = 0;
        public int ResumeCount { get; set; } = 0;
        public DateTime LastActivityTime { get; set; } = DateTime.UtcNow;
        
        // Calculated properties
        public int ReadingTimeMinutes => ReadingTimeSeconds / 60;
        public bool IsActiveSession => EndTime == null && 
                                      DateTime.UtcNow.Subtract(LastActivityTime).TotalMinutes < 30;
    }
}