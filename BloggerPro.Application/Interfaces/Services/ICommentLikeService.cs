using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface ICommentLikeService
    {
        Task<IResult> LikeCommentAsync(Guid commentId, Guid userId);
        Task<IResult> UnlikeCommentAsync(Guid commentId, Guid userId);
        Task<DataResult<int>> GetLikeCountAsync(Guid commentId);
        Task<DataResult<bool>> HasUserLikedAsync(Guid commentId, Guid userId);
    }
}
