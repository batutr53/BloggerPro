using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentLikeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> LikeCommentAsync(Guid commentId, Guid userId)
        {
            var exists = await _unitOfWork.CommentLikes
                .Query()
                .AnyAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

            if (exists)
                return new ErrorResult("Zaten beğenmişsiniz.");

            var like = new CommentLike
            {
                CommentId = commentId,
                UserId = userId
            };

            await _unitOfWork.CommentLikes.AddAsync(like);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Yorum beğenildi.");
        }

        public async Task<IResult> UnlikeCommentAsync(Guid commentId, Guid userId)
        {
            var like = await _unitOfWork.CommentLikes
                .Query()
                .FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

            if (like == null)
                return new ErrorResult("Beğeni bulunamadı.");

            await _unitOfWork.CommentLikes.DeleteAsync(like);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Beğeni geri alındı.");
        }

        public async Task<DataResult<int>> GetLikeCountAsync(Guid commentId)
        {
            var count = await _unitOfWork.CommentLikes
                .Query()
                .CountAsync(cl => cl.CommentId == commentId);

            return new SuccessDataResult<int>(count);
        }

        public async Task<DataResult<bool>> HasUserLikedAsync(Guid commentId, Guid userId)
        {
            var liked = await _unitOfWork.CommentLikes
                .Query()
                .AnyAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

            return new SuccessDataResult<bool>(liked);
        }
    }
}
