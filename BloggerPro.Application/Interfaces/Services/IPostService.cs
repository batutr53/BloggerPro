using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IPostService
{
    Task<DataResult<Guid>> CreatePostAsync(PostCreateDto dto, Guid authorId);
    Task<Result> DeletePostAsync(Guid id);
    Task<DataResult<PostDetailDto>> GetPostBySlugAsync(string slug);
    Task<DataResult<List<PostListDto>>> GetPostsPagedAsync(int page = 1, int pageSize = 10);
    Task<DataResult<List<PostListDto>>> FilterPostsAsync(PostFilterDto filterDto);
    Task<DataResult<PostWithModulesDto>> GetPostWithModulesAsync(Guid postId);
    Task<IResult> ReorderModulesAsync(Guid postId, List<ModuleSortOrderDto> newOrder);

    Task<Result> UpdatePostAsync(PostUpdateDto dto, Guid userId);




}
