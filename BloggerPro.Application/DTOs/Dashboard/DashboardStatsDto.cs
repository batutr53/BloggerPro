using BloggerPro.Application.DTOs.Post;

namespace BloggerPro.Application.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }
        public int TotalRatings { get; set; }

        public List<PostListDto> TopLikedPosts { get; set; } = new();
        public List<PostListDto> TopRatedPosts { get; set; } = new();
        public List<UserSummaryDto> MostActiveUsers { get; set; } = new();
    }

}
