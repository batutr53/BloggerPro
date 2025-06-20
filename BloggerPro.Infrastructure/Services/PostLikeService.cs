using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class PostLikeService : IPostLikeService
{
    private readonly IUnitOfWork _uow;

    public PostLikeService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IResult> LikePostAsync(Guid postId, Guid userId)
    {
        var exists = await _uow.Posts.Query()
            .Include(p => p.Likes)
            .AnyAsync(p => p.Id == postId && p.Likes.Any(l => l.UserId == userId));

        if (exists)
            return new ErrorResult("Zaten beğenmişsiniz.");

        var postLike = new PostLike
        {
            PostId = postId,
            UserId = userId
        };

        await _uow.PostLikes.AddAsync(postLike);
        await _uow.SaveChangesAsync();

        return new SuccessResult("Beğenildi.");
    }

    public async Task<IResult> UnlikePostAsync(Guid postId, Guid userId)
    {
        var like = await _uow.PostLikes.Query()
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

        if (like == null)
            return new ErrorResult("Beğeni bulunamadı.");

        await _uow.PostLikes.DeleteAsync(like);
        await _uow.SaveChangesAsync();

        return new SuccessResult("Beğeni geri alındı.");
    }

    public async Task<DataResult<int>> GetLikeCountAsync(Guid postId)
    {
        var count = await _uow.PostLikes.Query()
            .CountAsync(pl => pl.PostId == postId);

        return new SuccessDataResult<int>(count);
    }

    public async Task<DataResult<bool>> HasUserLikedAsync(Guid postId, Guid userId)
    {
        var liked = await _uow.PostLikes.Query()
            .AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);

        return new SuccessDataResult<bool>(liked);
    }
}
