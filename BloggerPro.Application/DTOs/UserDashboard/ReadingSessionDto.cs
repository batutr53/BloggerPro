namespace BloggerPro.Application.DTOs.UserDashboard
{
    public class ReadingSessionDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string PostSlug { get; set; } = string.Empty;
        public string PostImage { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int ReadingTimeSeconds { get; set; }
        public int ReadingTimeMinutes => ReadingTimeSeconds / 60;
        public bool IsCompleted { get; set; }
        public int ScrollPercentage { get; set; } // How much of the post was read
        public DateTime LastActivityTime { get; set; }
    }
}