using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModeration;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IAdminModerationService
    {
        Task<DataResult<List<PostModerationDto>>> GetAllPostsAsync();
        Task<Result> DeletePostAsync(Guid postId);
        Task<DataResult<PostDetailDto>> GetPostDetailsAsync(Guid postId);
    }

}
