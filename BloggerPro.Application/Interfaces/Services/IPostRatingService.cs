using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IPostRatingService
{
    Task<Result> RatePostAsync(Guid userId, Guid postId, int ratingValue);
    Task<DataResult<double>> GetAverageRatingAsync(Guid postId);
    Task<DataResult<int?>> GetUserRatingAsync(Guid postId, Guid userId);

}
