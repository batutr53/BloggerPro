using AutoMapper;
using BloggerPro.Application.DTOs.Comment;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class UserPanelService : IUserPanelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserPanelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DataResult<List<PostListDto>>> GetRatedPostsAsync(Guid userId)
        {
            var posts = await _unitOfWork.Posts.Query()
                .Where(p => p.Ratings.Any(r => r.UserId == userId))
                .ToListAsync();

            var dto = _mapper.Map<List<PostListDto>>(posts);
            return new SuccessDataResult<List<PostListDto>>(dto);
        }

        public async Task<DataResult<List<PostListDto>>> GetCommentedPostsAsync(Guid userId)
        {
            var posts = await _unitOfWork.Posts.Query()
                .Where(p => p.Comments.Any(c => c.UserId == userId))
                .ToListAsync();

            var dto = _mapper.Map<List<PostListDto>>(posts);
            return new SuccessDataResult<List<PostListDto>>(dto);
        }

        public async Task<DataResult<List<PostListDto>>> GetLikedPostsAsync(Guid userId)
        {
            var posts = await _unitOfWork.Posts.Query()
                .Where(p => p.Likes.Any(l => l.UserId == userId))
                .ToListAsync();

            var dto = _mapper.Map<List<PostListDto>>(posts);
            return new SuccessDataResult<List<PostListDto>>(dto);
        }

        public async Task<DataResult<List<CommentListDto>>> GetCommentsOnMyPostsAsync(Guid userId)
        {
            var comments = await _unitOfWork.Comments.Query()
                .Include(c => c.User)
                .Where(c => c.Post.AuthorId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var dto = _mapper.Map<List<CommentListDto>>(comments);
            return new SuccessDataResult<List<CommentListDto>>(dto);
        }
    }

}
