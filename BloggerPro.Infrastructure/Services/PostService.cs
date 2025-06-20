using AutoMapper;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Extensions;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PostService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IResult> ReorderModulesAsync(Guid postId, List<ModuleSortOrderDto> newOrder)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Modules)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            return new ErrorResult("Gönderi bulunamadı.");

        foreach (var module in post.Modules)
        {
            var newSort = newOrder.FirstOrDefault(o => o.Id == module.Id);
            if (newSort != null)
            {
                module.Order = newSort.SortOrder;
            }
        }

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Modül sıralaması güncellendi.");
    }



    public async Task<DataResult<Guid>> CreatePostAsync(PostCreateDto dto, Guid authorId)
    {
        var post = _mapper.Map<Post>(dto);
        post.AuthorId = authorId;

        if (dto.Modules != null && dto.Modules.Any())
        {
            post.Modules = dto.Modules.Select(m => new PostModule
            {
                Type = m.Type,
                Content = m.Content,
                MediaUrl = m.MediaUrl,
                Alignment = m.Alignment,
                Width = m.Width,
                SortOrder = m.SortOrder
            }).ToList();
        }

        await _unitOfWork.Posts.AddAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessDataResult<Guid>(post.Id, "Post başarıyla oluşturuldu.");
    }

    public async Task<DataResult<List<PostListDto>>> FilterPostsAsync(PostFilterDto dto)
    {
        var query = _unitOfWork.Posts.Query()
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.PostCategories)
            .Include(p => p.PostTags)
            .Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(dto.Keyword))
            query = query.Where(p => p.Title.Contains(dto.Keyword) || p.Content.Contains(dto.Keyword));

        if (dto.CategoryId.HasValue)
            query = query.Where(p => p.PostCategories.Any(pc => pc.CategoryId == dto.CategoryId));

        if (dto.TagId.HasValue)
            query = query.Where(p => p.PostTags.Any(pt => pt.TagId == dto.TagId));

        if (dto.MinRating.HasValue)
            query = query.Where(p => p.Ratings.Any() &&
                                     p.Ratings.Average(r => r.RatingValue) >= dto.MinRating);

        if (!string.IsNullOrEmpty(dto.SortBy))
        {
            query = dto.SortBy switch
            {
                "like" => query.OrderByDescending(p => p.Likes.Count),
                "comment" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };
        }

        var posts = await query
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        var result = _mapper.Map<List<PostListDto>>(posts);
        return new SuccessDataResult<List<PostListDto>>(result);
    }
    public async Task<DataResult<PostWithModulesDto>> GetPostWithModulesAsync(Guid postId)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Modules)
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            return new ErrorDataResult<PostWithModulesDto>("Post bulunamadı.");

        var dto = _mapper.Map<PostWithModulesDto>(post);
        return new SuccessDataResult<PostWithModulesDto>(dto);
    }


    public async Task<Result> UpdatePostAsync(PostUpdateDto dto, Guid userId)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Modules)
             .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == dto.Id && p.AuthorId == userId);

        if (post == null)
            return new ErrorResult("Post bulunamadı veya yetkiniz yok.");

        _mapper.Map(dto, post);
        post.PostTags.Clear();
        foreach (var tagId in dto.TagIds)
        {
            post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
        }

        // Mevcut modülleri sil
        post.Modules.Clear();

        if (dto.Modules != null && dto.Modules.Any())
        {
            post.Modules = dto.Modules.Select(m => new PostModule
            {
                Type = m.Type,
                Content = m.Content,
                MediaUrl = m.MediaUrl,
                Alignment = m.Alignment,
                Width = m.Width,
                SortOrder = m.SortOrder
            }).ToList();
        }

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Post güncellendi.");
    }


    public async Task<Result> DeletePostAsync(Guid id)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post is null)
            return new ErrorResult("Post bulunamadı.");

        await _unitOfWork.Posts.DeleteAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Post silindi.");
    }

    public async Task<DataResult<PostDetailDto>> GetPostBySlugAsync(string slug)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Author)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (post is null)
            return new ErrorDataResult<PostDetailDto>("Post bulunamadı.");

        var dto = _mapper.Map<PostDetailDto>(post);
        return new SuccessDataResult<PostDetailDto>(dto);
    }

    public async Task<DataResult<List<PostListDto>>> GetPostsPagedAsync(int page = 1, int pageSize = 10)
    {
        var posts = await _unitOfWork.Posts.Query()
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<PostListDto>>(posts);
        return new SuccessDataResult<List<PostListDto>>(dtos, dtos.Count);
    }
}
