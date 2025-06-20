using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class PostRatingService : IPostRatingService
{
    private readonly IUnitOfWork _uow;

    public PostRatingService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> RatePostAsync(Guid userId, Guid postId, int ratingValue)
    {
        var existingRating = await _uow.PostRatings.Query()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.PostId == postId);

        if (existingRating is not null)
        {
            existingRating.RatingValue = ratingValue;
            existingRating.RatedAt = DateTime.UtcNow;
            await _uow.PostRatings.UpdateAsync(existingRating);
        }
        else
        {
            var rating = new PostRating
            {
                UserId = userId,
                PostId = postId,
                RatingValue = ratingValue,
                RatedAt = DateTime.UtcNow

            };
            await _uow.PostRatings.AddAsync(rating);
        }

        await _uow.SaveChangesAsync();
        return new SuccessResult("Puanlama başarılı.");
    }

    public async Task<DataResult<double>> GetAverageRatingAsync(Guid postId)
    {
        var ratings = await _uow.PostRatings.Query()
            .Where(r => r.PostId == postId)
            .ToListAsync();

        if (!ratings.Any())
            return new ErrorDataResult<double>("Henüz puan verilmemiş.");

        var average = ratings.Average(r => r.RatingValue);
        return new SuccessDataResult<double>(average);
    }
    public async Task<DataResult<int?>> GetUserRatingAsync(Guid postId, Guid userId)
    {
        var rating = await _uow.PostRatings.Query()
            .Where(r => r.PostId == postId && r.UserId == userId)
            .Select(r => (int?)r.RatingValue)
            .FirstOrDefaultAsync();

        return new SuccessDataResult<int?>(rating);
    }
}
