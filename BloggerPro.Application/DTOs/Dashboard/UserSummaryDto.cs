namespace BloggerPro.Application.DTOs.Dashboard
{
    public class UserSummaryDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalComments { get; set; }
        public int TotalPosts { get; set; }
        public int TotalRatings { get; set; }
    }
}
