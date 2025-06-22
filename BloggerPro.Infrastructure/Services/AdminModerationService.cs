using AutoMapper;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModeration;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class AdminModerationService : IAdminModerationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminModerationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DataResult<List<PostModerationDto>>> GetAllPostsAsync()
        {
            var posts = await _unitOfWork.Posts.Query()
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Ratings)
                .Include(p => p.Author)
                .ToListAsync();

            var result = posts.Select(p => new PostModerationDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                AuthorName = p.Author.UserName,
                CommentCount = p.Comments.Count,
                LikeCount = p.Likes.Count,
                RatingCount = p.Ratings.Count,
                CreatedAt = p.CreatedAt
            }).ToList();

            return new SuccessDataResult<List<PostModerationDto>>(result);
        }

        public async Task<DataResult<PostDetailDto>> GetPostDetailsAsync(Guid postId)
        {
            var post = await _unitOfWork.Posts.Query()
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return new ErrorDataResult<PostDetailDto>("Post bulunamadı.");

            var dto = _mapper.Map<PostDetailDto>(post);
            return new SuccessDataResult<PostDetailDto>(dto);
        }

        public async Task<Result> DeletePostAsync(Guid postId)
        {
            var post = await _unitOfWork.Posts.Query()
      .Include(p => p.Modules)
          .ThenInclude(m => m.SeoMetadata)
      .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return new ErrorResult("Post bulunamadı.");

            // SeoMetadata'ları sil
            var allSeoMetadata = post.Modules
                .SelectMany(m => m.SeoMetadata)
                .ToList();

             _unitOfWork.SeoMetadatas.DeleteRange(allSeoMetadata);
            
            // PostModules'ları sil
            _unitOfWork.PostModules.DeleteRange(post.Modules);

            // Son olarak postu sil
            await _unitOfWork.Posts.DeleteAsync(post);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Post silindi.");

        }
    }

}
