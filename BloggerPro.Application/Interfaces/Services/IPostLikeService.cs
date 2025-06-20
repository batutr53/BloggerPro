using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IPostLikeService
{
    Task<IResult> LikePostAsync(Guid postId, Guid userId);
    Task<IResult> UnlikePostAsync(Guid postId, Guid userId);
    Task<DataResult<int>> GetLikeCountAsync(Guid postId);
    Task<DataResult<bool>> HasUserLikedAsync(Guid postId, Guid userId);
}
