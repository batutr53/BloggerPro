using AutoMapper;
using BloggerPro.Application.DTOs.Pagination;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Application.DTOs.SeoMetadata;
using BloggerPro.Application.Interfaces;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Constants;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Enums;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Extensions;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public PostService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<User> userManager,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }
    // Post Status Management
    public async Task<Result> UpdatePostStatusAsync(Guid postId, PostStatus status, Guid userId, DateTime? publishDate = null)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            return new ErrorResult("Post not found");

        if (!await CanUserEditPostAsync(postId, userId))
            return new ErrorResult("You don't have permission to update this post's status");

        post.Status = status;
        if (status == PostStatus.Published)
        {
            post.PublishDate = publishDate ?? DateTime.UtcNow;
        }

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();


        return new SuccessResult($"Post status updated to {status}");
    }

    public async Task<Result> UpdatePostVisibilityAsync(Guid postId, PostVisibility visibility, Guid userId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            return new ErrorResult("Post not found");

        if (!await CanUserEditPostAsync(postId, userId))
            return new ErrorResult("You don't have permission to update this post's visibility");

        post.Visibility = visibility;
        post.LastModified = DateTime.UtcNow;

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult($"Post visibility updated to {visibility}");
    }

    // Permission checks
    public async Task<bool> CanUserEditPostAsync(Guid postId, Guid userId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null) return false;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var userRoles = await _userManager.GetRolesAsync(user);

        // Admin can edit any post
        if (userRoles.Contains(UserRoles.Admin))
            return true;

        // Editor can edit any post if they have the role
        if (userRoles.Contains(UserRoles.Editor))
            return true;

        // Regular users can only edit their own posts
        return post.AuthorId == userId;
    }

    public async Task<bool> IsPostOwnerAsync(Guid postId, Guid userId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        return post?.AuthorId == userId;
    }

    public async Task<bool> CanUserViewPostAsync(Guid postId, Guid? userId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null) return false;

        // If post is published and public, anyone can view it
        if (post.Status == PostStatus.Published && post.Visibility == PostVisibility.Public)
            return true;

        // If no user is logged in, can't view non-public posts
        if (!userId.HasValue)
            return false;

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null) return false;

        // Admin can view any post
        if (await _userManager.IsInRoleAsync(user, UserRoles.Admin))
            return true;

        // Post owner can view their own post
        if (post.AuthorId == userId)
            return true;

        // Check for followers-only visibility
        if (post.Visibility == PostVisibility.FollowersOnly)
        {
            var isFollowing = await _unitOfWork.UserFollowers.AnyAsync(uf =>
                uf.FollowerId == userId && uf.FollowingId == post.AuthorId);
            return isFollowing;
        }

        return false;
    }

    public async Task<Result> ReorderModulesAsync(Guid postId, List<ModuleSortOrderDto> newOrder, Guid userId)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Modules)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            return new ErrorResult("Post not found");


        if (!await CanUserEditPostAsync(postId, userId))
            return new ErrorResult("You don't have permission to reorder modules for this post");

        // Validate that all module IDs exist and belong to this post
        var moduleIds = post.Modules.Select(m => m.Id).ToList();
        if (newOrder.Any(o => !moduleIds.Contains(o.Id)))
            return new ErrorResult("One or more module IDs are invalid");

        // Update module orders
        foreach (var module in post.Modules)
        {
            var newSort = newOrder.FirstOrDefault(o => o.Id == module.Id);
            if (newSort != null)
            {
                module.Order = newSort.SortOrder;
                module.UpdatedAt = DateTime.UtcNow;
            }
        }

        post.LastModified = DateTime.UtcNow;

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Module order updated successfully");
    }



    public async Task<DataResult<Guid>> CreatePostAsync(PostCreateDto dto, Guid authorId)
    {
        // Kullanıcı kontrolü
        var user = await _userManager.FindByIdAsync(authorId.ToString());
        if (user == null)
            return new ErrorDataResult<Guid>("User not found");

        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Any(r => r == UserRoles.Admin || r == UserRoles.Editor))
            return new ErrorDataResult<Guid>("You don't have permission to create posts");

        // Post oluştur
        var post = _mapper.Map<Post>(dto);
        post.AuthorId = authorId;
        post.Status = PostStatus.Draft;
        post.Visibility = PostVisibility.Public;
        post.CreatedAt = DateTime.UtcNow;
        post.LastModified = DateTime.UtcNow;
        post.Excerpt = ContentHelper.GenerateExcerpt(dto.Content);

        // Modüller ve otomatik SEO Metadata
        if (dto.Modules != null && dto.Modules.Any())
        {
            post.Modules = dto.Modules.Select(m =>
            {
                var module = new PostModule
                {
                    Type = m.Type,
                    Content = m.Content,
                    MediaUrl = m.MediaUrl,
                    Alignment = m.Alignment,
                    Width = m.Width,
                    SortOrder = m.SortOrder,
                    SeoMetadata = new List<SeoMetadata>()
                };

                if (m.SeoMetadata != null && m.SeoMetadata.Any())
                {
                    module.SeoMetadata = m.SeoMetadata.Select(s => new SeoMetadata
                    {
                        Title = s.Title ?? ContentHelper.GenerateTitle(m.Content),
                        Description = s.Description ?? ContentHelper.GenerateExcerpt(m.Content),
                        Keywords = s.Keywords ?? "",
                        LanguageCode = s.LanguageCode ?? "tr",
                        CanonicalGroupId = s.CanonicalGroupId != Guid.Empty ? s.CanonicalGroupId : Guid.NewGuid()
                    }).ToList();
                }
                else
                {
                    module.SeoMetadata = new List<SeoMetadata>
                {
                    new SeoMetadata
                    {
                        Title = ContentHelper.GenerateTitle(m.Content),
                        Description = ContentHelper.GenerateExcerpt(m.Content),
                        Keywords = "",
                        LanguageCode = "tr",
                        CanonicalGroupId = Guid.NewGuid()
                    }
                };
                }

                return module;
            }).ToList();
        }

        // Kategoriler
        if (dto.CategoryIds != null && dto.CategoryIds.Any())
        {
            post.PostCategories = dto.CategoryIds.Select(categoryId => new PostCategory
            {
                PostId = post.Id,
                CategoryId = categoryId
            }).ToList();
        }

        // Etiketler ✅ Eksik olan kısım eklendi
        if (dto.TagIds != null && dto.TagIds.Any())
        {
            post.PostTags = dto.TagIds.Select(tagId => new PostTag
            {
                PostId = post.Id,
                TagId = tagId
            }).ToList();
        }

        await _unitOfWork.Posts.AddAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessDataResult<Guid>(post.Id, "Post created successfully");
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

        if (dto.CategoryIds != null && dto.CategoryIds.Any())
            query = query.Where(p => p.PostCategories.Any(c => dto.CategoryIds.Contains(c.CategoryId)));

        if (dto.TagIds != null && dto.TagIds.Any())
        {
            query = query.Where(p => p.PostTags.Any(t => dto.TagIds.Contains(t.TagId)));
        }

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
                .ThenInclude(m => m.SeoMetadata)
            .Include(p => p.PostTags)
            .Include(p => p.PostCategories)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (post == null)
            return new ErrorResult("Post not found");

        if (!await CanUserEditPostAsync(dto.Id, userId))
            return new ErrorResult("You don't have permission to edit this post");

        // Post ana alanlarını güncelle
        post.Title = dto.Title;
        post.Slug = dto.Slug;
        post.Excerpt = dto.Excerpt;
        post.Content = dto.Content;
        post.FeaturedImage = dto.CoverImageUrl;
        post.AllowComments = dto.AllowComments;
        post.IsFeatured = dto.IsFeatured;
        post.Status = (PostStatus)dto.Status;
        post.Visibility = (PostVisibility)dto.Visibility;
        post.PublishDate = dto.PublishDate;
        post.LastModified = DateTime.UtcNow;

        // Etiketleri güncelle
        _unitOfWork.PostTags.DeleteRange(post.PostTags);
        post.PostTags = dto.TagIds?.Select(tagId => new PostTag
        {
            PostId = post.Id,
            TagId = tagId
        }).ToList() ?? new List<PostTag>();

        // Kategorileri güncelle
        _unitOfWork.PostCategories.DeleteRange(post.PostCategories);
        post.PostCategories = dto.CategoryIds?.Select(catId => new PostCategory
        {
            PostId = post.Id,
            CategoryId = catId
        }).ToList() ?? new List<PostCategory>();

        // Modülleri ve SEO verilerini sil
        foreach (var oldModule in post.Modules.ToList())
        {
            if (oldModule.SeoMetadata.Any())
                _unitOfWork.SeoMetadatas.DeleteRange(oldModule.SeoMetadata);

            await _unitOfWork.PostModules.DeleteAsync(oldModule);
        }
        post.Modules.Clear(); // Temizleme işlemi

        // Yeni modülleri ve SEO'ları ekle
        if (dto.Modules != null && dto.Modules.Any())
        {
            foreach (var moduleDto in dto.Modules)
            {
                var moduleId = Guid.NewGuid();

                var module = new PostModule
                {
                    Id = moduleId,
                    PostId = post.Id,
                    Type = moduleDto.Type,
                    Content = moduleDto.Content,
                    MediaUrl = moduleDto.MediaUrl,
                    Order = moduleDto.Order,
                    SortOrder = moduleDto.SortOrder,
                    Alignment = moduleDto.Alignment,
                    Width = moduleDto.Width
                };

                await _unitOfWork.PostModules.AddAsync(module);

                var metaList = moduleDto.SeoMetadata?.Select(meta => new SeoMetadata
                {
                    Title = meta.Title ?? ContentHelper.GenerateTitle(moduleDto.Content),
                    Description = meta.Description ?? ContentHelper.GenerateExcerpt(moduleDto.Content),
                    Keywords = meta.Keywords ?? "",
                    LanguageCode = meta.LanguageCode ?? "tr",
                    CanonicalGroupId = meta.CanonicalGroupId != Guid.Empty ? meta.CanonicalGroupId : Guid.NewGuid(),
                    PostModuleId = moduleId
                }).ToList() ?? new List<SeoMetadata>();

                if (metaList.Any())
                    await _unitOfWork.SeoMetadatas.AddRangeAsync(metaList);
            }
        }

        _unitOfWork.Posts.Update(post);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Post updated successfully");
    }


    public async Task<Result> DeletePostAsync(Guid id, Guid userId)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Modules)
                .ThenInclude(m => m.SeoMetadata)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return new ErrorResult("Post not found");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        var userRoles = await _userManager.GetRolesAsync(user);

        if (post.AuthorId != userId && !userRoles.Contains(UserRoles.Admin) && !userRoles.Contains(UserRoles.Editor))
            return new ErrorResult("You don't have permission to delete this post");

        // Önce SEO metadata'ları sil
        foreach (var module in post.Modules)
        {
            if (module.SeoMetadata.Any())
                _unitOfWork.SeoMetadatas.DeleteRange(module.SeoMetadata);
        }

        // Sonra modülleri sil
        _unitOfWork.PostModules.DeleteRange(post.Modules);

        // En son post'u sil
        await _unitOfWork.Posts.DeleteAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Post deleted successfully");
    }


    public async Task<DataResult<PostDetailDto>> GetPostByIdAsync(Guid id, Guid? userId = null)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Author)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Likes)
            .Include(p => p.Ratings)
            .Include(p => p.Modules)
                .ThenInclude(m => m.SeoMetadata) // Eksik olan kısım
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return new ErrorDataResult<PostDetailDto>("Post not found");

        // Kullanıcının görüntüleme yetkisi var mı?
        if (!await CanUserViewPostAsync(post.Id, userId))
            return new ErrorDataResult<PostDetailDto>("You don't have permission to view this post");

        // DTO'ya mapleme
        var dto = _mapper.Map<PostDetailDto>(post);

        // Kategoriler ve etiketler
        dto.Categories = post.PostCategories?
            .Select(pc => pc.Category?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList() ?? new List<string>();

        dto.Tags = post.PostTags?
            .Select(pt => pt.Tag?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList() ?? new List<string>();

        // Kullanıcıya özel veriler (like ve rating)
        if (userId.HasValue)
        {
            dto.IsLikedByCurrentUser = await _unitOfWork.PostLikes
                .AnyAsync(pl => pl.PostId == id && pl.UserId == userId.Value);

            var userRating = await _unitOfWork.PostRatings
                .Query()
                .FirstOrDefaultAsync(pr => pr.PostId == id && pr.UserId == userId.Value);

            if (userRating != null)
            {
                dto.CurrentUserRating = userRating.RatingValue;
            }
        }

        return new SuccessDataResult<PostDetailDto>(dto);
    }


    public async Task<DataResult<Application.DTOs.Pagination.PaginatedResultDto<PostListDto>>> GetPostsByAuthorIdAsync(Guid authorId, PostFilterDto filter, int page = 1, int pageSize = 10)
    {
        var query = _unitOfWork.Posts.Query()
            .Where(p => p.AuthorId == authorId);

        // Apply filters
        if (!string.IsNullOrEmpty(filter.Keyword))
            query = query.Where(p => p.Title.Contains(filter.Keyword) || p.Content.Contains(filter.Keyword));

        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);

        if (filter.CategoryIds != null && filter.CategoryIds.Any())
            query = query.Where(p => p.PostCategories.Any(pc => filter.CategoryIds.Contains(pc.CategoryId)));

        if (filter.TagIds != null && filter.TagIds.Any())
            query = query.Where(p => p.PostTags.Any(pt => filter.TagIds.Contains(pt.TagId)));

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.SortDescending 
                ? query.OrderByDescending(p => p.Title) 
                : query.OrderBy(p => p.Title),
            "views" => filter.SortDescending 
                ? query.OrderByDescending(p => p.ViewCount) 
                : query.OrderBy(p => p.ViewCount),
            _ => filter.SortDescending 
                ? query.OrderByDescending(p => p.CreatedAt) 
                : query.OrderBy(p => p.CreatedAt)
        };

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var posts = await query
         .Include(p => p.Comments)
         .Include(p => p.Likes)
         .Include(p => p.PostCategories)
             .ThenInclude(pc => pc.Category)
         .Include(p => p.PostTags)
             .ThenInclude(pt => pt.Tag)
         .Skip((page - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync();

        var dtos = _mapper.Map<List<PostListDto>>(posts);
        var paginatedResult = new PaginatedResultDto<PostListDto>(dtos, totalCount, page, pageSize);
        
        return new SuccessDataResult<PaginatedResultDto<PostListDto>>(paginatedResult);
    }


    public async Task<DataResult<PaginatedResultDto<PostListDto>>> GetAllPostsAsync(int page = 1, int pageSize = 20)
    {
        var query = _unitOfWork.Posts.Query();

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var posts = await query
            .Include(p => p.Modules)
                .ThenInclude(m => m.SeoMetadata)
            .Include(p => p.Author)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.CreatedAt)

            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<PostListDto>>(posts);
        var paginatedResult = new PaginatedResultDto<PostListDto>(dtos, totalCount, page, pageSize);

        return new SuccessDataResult<PaginatedResultDto<PostListDto>>(paginatedResult);
    }

    public async Task<DataResult<Application.DTOs.Pagination.PaginatedResultDto<PostListDto>>> GetAllPostsAsync(PostFilterDto filter, int page = 1, int pageSize = 10)
    {
        var query = _unitOfWork.Posts.Query()
            .Where(p => p.IsActive);

        // Apply filters
        if (!string.IsNullOrEmpty(filter.Keyword))
            query = query.Where(p => p.Title.Contains(filter.Keyword) || p.Content.Contains(filter.Keyword));

        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status);

        if (filter.AuthorId.HasValue)
            query = query.Where(p => p.AuthorId == filter.AuthorId);

        if (filter.IsFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == filter.IsFeatured);

        if (filter.Visibility.HasValue)
            query = query.Where(p => p.Visibility == filter.Visibility);

        if (filter.FromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(p => p.CreatedAt <= filter.ToDate.Value);

        if (filter.CategoryIds != null && filter.CategoryIds.Any())
            query = query.Where(p => p.PostCategories.Any(pc => filter.CategoryIds.Contains(pc.CategoryId)));

        if (filter.TagIds != null && filter.TagIds.Any())
            query = query.Where(p => p.PostTags.Any(pt => filter.TagIds.Contains(pt.TagId)));

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.SortDescending 
                ? query.OrderByDescending(p => p.Title) 
                : query.OrderBy(p => p.Title),
            "views" => filter.SortDescending 
                ? query.OrderByDescending(p => p.ViewCount) 
                : query.OrderBy(p => p.ViewCount),
            "likes" => filter.SortDescending 
                ? query.OrderByDescending(p => p.Likes.Count) 
                : query.OrderBy(p => p.Likes.Count),
            "comments" => filter.SortDescending 
                ? query.OrderByDescending(p => p.Comments.Count) 
                : query.OrderBy(p => p.Comments.Count),
            "rating" => filter.SortDescending 
                ? query.OrderByDescending(p => p.AverageRating) 
                : query.OrderBy(p => p.AverageRating),
            _ => filter.SortDescending 
                ? query.OrderByDescending(p => p.CreatedAt) 
                : query.OrderBy(p => p.CreatedAt)
        };

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var posts = await query
            .Include(p => p.Author)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<PostListDto>>(posts);
        var paginatedResult = new PaginatedResultDto<PostListDto>(dtos, totalCount, page, pageSize);
        
        return new SuccessDataResult<PaginatedResultDto<PostListDto>>(paginatedResult);
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

    public async Task<Result> TogglePostFeaturedStatusAsync(Guid postId, Guid userId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            return new ErrorResult("Post not found");

        if (!await CanUserEditPostAsync(postId, userId))
            return new ErrorResult("You don't have permission to update this post's featured status");

        post.IsFeatured = !post.IsFeatured;
        post.LastModified = DateTime.UtcNow;

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult($"Post featured status toggled to {post.IsFeatured}");
    }

    public async Task<DataResult<PostModuleDto>> AddModuleToPostAsync(Guid postId, CreatePostModuleDto dto, Guid userId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            return new ErrorDataResult<PostModuleDto>("Post not found");

        if (!await CanUserEditPostAsync(postId, userId))
            return new ErrorDataResult<PostModuleDto>("You don't have permission to add modules to this post");

        var module = new PostModule
        {
            PostId = postId,
            Type = dto.Type,
            Content = dto.Content,
            MediaUrl = dto.MediaUrl,
            Order = dto.Order,
            SortOrder = dto.Order,
            Alignment = dto.Alignment,
            Width = dto.Width,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.PostModules.AddAsync(module);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<PostModuleDto>(module);
        return new SuccessDataResult<PostModuleDto>(result, "Module added successfully");
    }

    public async Task<DataResult<PostModuleDto>> UpdateModuleAsync(Guid postId, UpdatePostModuleDto dto, Guid userId)
    {
        var module = await _unitOfWork.PostModules.GetByIdAsync(dto.Id);
        if (module == null || module.PostId != postId)
            return new ErrorDataResult<PostModuleDto>("Module not found or does not belong to the specified post");

        if (!await CanUserEditPostAsync(postId, userId))
            return new ErrorDataResult<PostModuleDto>("You don't have permission to update this module");

        _mapper.Map(dto, module);
        module.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.PostModules.UpdateAsync(module);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<PostModuleDto>(module);
        return new SuccessDataResult<PostModuleDto>(result, "Module updated successfully");
    }

    public async Task<Result> RemoveModuleFromPostAsync(Guid postId, Guid moduleId, Guid userId)
    {
        var module = await _unitOfWork.PostModules.GetByIdAsync(moduleId);
        if (module == null || module.PostId != postId)
            return new ErrorResult("Module not found or does not belong to the specified post");

        if (!await CanUserEditPostAsync(postId, userId))
            return new ErrorResult("You don't have permission to remove this module");

        await _unitOfWork.PostModules.DeleteAsync(module);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Module removed successfully");
    }

    public async Task<Result> LikePostAsync(Guid postId, Guid userId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            return new ErrorResult("Post not found");

        var existingLike = await _unitOfWork.PostLikes
            .FindByCondition(pl => pl.PostId == postId && pl.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingLike != null)
            return new ErrorResult("You have already liked this post");

        var like = new PostLike
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.PostLikes.AddAsync(like);
        post.LikeCount++;
        
        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Post liked successfully");
    }

    public async Task<Result> UnlikePostAsync(Guid postId, Guid userId)
    {
        var like = await _unitOfWork.PostLikes
            .FindByCondition(pl => pl.PostId == postId && pl.UserId == userId)
            .FirstOrDefaultAsync();

        if (like == null)
            return new ErrorResult("Like not found");

        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post != null && post.LikeCount > 0)
        {
            post.LikeCount--;
            await _unitOfWork.Posts.UpdateAsync(post);
        }

        await _unitOfWork.PostLikes.DeleteAsync(like);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Post unliked successfully");
    }

    public async Task<Result> RatePostAsync(Guid postId, int score, Guid userId)
    {
        if (score < 1 || score > 5)
            return new ErrorResult("Rating must be between 1 and 5");

        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            return new ErrorResult("Post not found");

        var existingRating = await _unitOfWork.PostRatings
            .FindByCondition(pr => pr.PostId == postId && pr.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingRating != null)
        {
            // Update existing rating
            existingRating.RatingValue = score;
            existingRating.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.PostRatings.UpdateAsync(existingRating);
        }
        else
        {
            // Create new rating
            var rating = new PostRating
            {
                PostId = postId,
                UserId = userId,
                RatingValue = score,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.PostRatings.AddAsync(rating);
        }

        // Update post rating stats
        var ratings = await _unitOfWork.PostRatings
            .FindByCondition(pr => pr.PostId == postId)
            .ToListAsync();

        post.AverageRating = ratings.Average(r => r.RatingValue);
        post.RatingCount = ratings.Count;
        
        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Post rated successfully");
    }

    public async Task<Result> RemoveRatingAsync(Guid postId, Guid userId)
    {
        var rating = await _unitOfWork.PostRatings
            .FindByCondition(pr => pr.PostId == postId && pr.UserId == userId)
            .FirstOrDefaultAsync();

        if (rating == null)
            return new ErrorResult("Rating not found");

        await _unitOfWork.PostRatings.DeleteAsync(rating);

        // Update post rating stats
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post != null)
        {
            var ratings = await _unitOfWork.PostRatings
                .FindByCondition(pr => pr.PostId == postId)
                .ToListAsync();

            post.RatingCount = ratings.Count;
            post.AverageRating = ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;
            
            await _unitOfWork.Posts.UpdateAsync(post);
            await _unitOfWork.SaveChangesAsync();
        }

        return new SuccessResult("Rating removed successfully");
    }

    public async Task<DataResult<PostStatsDto>> GetPostStatsAsync(Guid postId, Guid userId)
    {
        var post = await _unitOfWork.Posts.Query()
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Ratings)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            return new ErrorDataResult<PostStatsDto>("Post not found");

        if (!await CanUserViewPostAsync(postId, userId))
            return new ErrorDataResult<PostStatsDto>("You don't have permission to view this post's stats");

        var stats = new PostStatsDto
        {
            ViewCount = post.ViewCount,
            LikeCount = post.LikeCount,
            CommentCount = post.Comments?.Count ?? 0,
            AverageRating = post.AverageRating,
            RatingCount = post.RatingCount,
            RatingDistribution = post.Ratings?.GroupBy(r => r.RatingValue)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count()) ?? new Dictionary<int, int>()
        };

        return new SuccessDataResult<PostStatsDto>(stats);
    }

    public async Task<DataResult<UserPostStatsDto>> GetUserPostStatsAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return new ErrorDataResult<UserPostStatsDto>("User not found");

        var posts = await _unitOfWork.Posts.FindByCondition(p => p.AuthorId == userId).ToListAsync();
        
        var stats = new UserPostStatsDto
        {
            TotalPosts = posts.Count,
            PublishedPosts = posts.Count(p => p.Status == PostStatus.Published),
            DraftPosts = posts.Count(p => p.Status == PostStatus.Draft),
            ArchivedPosts = posts.Count(p => p.Status == PostStatus.Archived),
            TotalLikesReceived = posts.Sum(p => p.LikeCount),
            TotalCommentsReceived = posts.Sum(p => p.Comments?.Count ?? 0),
            AveragePostRating = posts.Any() ? posts.Average(p => p.AverageRating ?? 0) : 0
        };

        return new SuccessDataResult<UserPostStatsDto>(stats);
    }
}
