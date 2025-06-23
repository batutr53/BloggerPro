using BloggerPro.Application.DTOs.Pagination;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Domain.Constants;
using BloggerPro.Domain.Enums;
using BloggerPro.Shared.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<DataResult<PaginatedResultDto<PostListDto>>> GetAllPostsAsync(int page = 1, int pageSize = 20);
        // Post CRUD Operations
        Task<DataResult<Guid>> CreatePostAsync(PostCreateDto dto, Guid authorId);
        Task<DataResult<PostDetailDto>> GetPostByIdAsync(Guid id, Guid? userId = null);
        Task<DataResult<PaginatedResultDto<PostListDto>>> GetPostsByAuthorIdAsync(Guid authorId, PostFilterDto filter, int page = 1, int pageSize = 10);
        Task<DataResult<PaginatedResultDto<PostListDto>>> GetAllPostsAsync(PostFilterDto filter, int page = 1, int pageSize = 10);
        Task<Result> UpdatePostAsync(PostUpdateDto dto, Guid userId);
        Task<Result> DeletePostAsync(Guid id, Guid userId);
        
        // Post Status Management
        Task<Result> UpdatePostStatusAsync(Guid postId, PostStatus status, Guid userId, DateTime? publishDate = null);
        Task<Result> UpdatePostVisibilityAsync(Guid postId, PostVisibility visibility, Guid userId);
        Task<Result> TogglePostFeaturedStatusAsync(Guid postId, Guid userId);
        
        // Post Modules
        Task<DataResult<PostModuleDto>> AddModuleToPostAsync(Guid postId, CreatePostModuleDto dto, Guid userId);
        Task<DataResult<PostModuleDto>> UpdateModuleAsync(Guid postId, UpdatePostModuleDto dto, Guid userId);
        Task<Result> RemoveModuleFromPostAsync(Guid postId, Guid moduleId, Guid userId);
        Task<Result> ReorderModulesAsync(Guid postId, List<ModuleSortOrderDto> newOrder, Guid userId);
        
        // Post Interactions
        Task<Result> LikePostAsync(Guid postId, Guid userId);
        Task<Result> UnlikePostAsync(Guid postId, Guid userId);
        Task<Result> RatePostAsync(Guid postId, int score, Guid userId);
        Task<Result> RemoveRatingAsync(Guid postId, Guid userId);
        
        // Permissions
        Task<bool> CanUserEditPostAsync(Guid postId, Guid userId);
        Task<bool> IsPostOwnerAsync(Guid postId, Guid userId);
        Task<bool> CanUserViewPostAsync(Guid postId, Guid? userId);
        
        // Stats
        Task<DataResult<PostStatsDto>> GetPostStatsAsync(Guid postId, Guid userId);
        Task<DataResult<UserPostStatsDto>> GetUserPostStatsAsync(Guid userId);
    }
    
    public class PostStatsDto
    {
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public double? AverageRating { get; set; }
        public int RatingCount { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
    
    public class UserPostStatsDto
    {
        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int DraftPosts { get; set; }
        public int ArchivedPosts { get; set; }
        public int TotalLikesReceived { get; set; }
        public int TotalCommentsReceived { get; set; }
        public double? AveragePostRating { get; set; }
    }
}
