using BloggerPro.Application.DTOs.Comment;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IUserPanelService
    {
        Task<DataResult<List<PostListDto>>> GetRatedPostsAsync(Guid userId);
        Task<DataResult<List<PostListDto>>> GetCommentedPostsAsync(Guid userId);
        Task<DataResult<List<PostListDto>>> GetLikedPostsAsync(Guid userId);
        Task<DataResult<List<CommentListDto>>> GetCommentsOnMyPostsAsync(Guid userId);
    }

}
