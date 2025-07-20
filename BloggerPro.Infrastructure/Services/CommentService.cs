using AutoMapper;
using BloggerPro.Application.DTOs.Comment;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DataResult<Guid>> AddCommentAsync(CommentCreateDto dto, Guid userId)
    {
        var comment = new Comment
        {
            Content = dto.Content,
            PostId = dto.PostId,
            UserId = userId,
            ParentCommentId = dto.ParentCommentId
        };

        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessDataResult<Guid>(comment.Id, "Yorum eklendi.");
    }

    public async Task<Result> DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment is null)
            return new ErrorResult("Yorum bulunamadı.");

        if (comment.UserId != userId)
            return new ErrorResult("Bu yorumu silmeye yetkiniz yok.");

        await _unitOfWork.Comments.DeleteAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Yorum silindi.");
    }

    public async Task<DataResult<List<CommentListDto>>> GetCommentsByPostAsync(Guid postId, Guid? currentUserId = null)
    {
        // Get all comments for the post (including nested ones)
        var allComments = await _unitOfWork.Comments.Query()
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .Include(c => c.Likes)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        // Build the hierarchical structure
        var topLevelComments = allComments
            .Where(c => c.ParentCommentId == null)
            .ToList();

        // Recursively load replies for each comment
        foreach (var comment in topLevelComments)
        {
            LoadRepliesRecursively(comment, allComments);
        }

        var dto = _mapper.Map<List<CommentListDto>>(topLevelComments);
        
        // Set HasLiked property for all comments and replies recursively
        if (currentUserId.HasValue)
        {
            SetHasLikedRecursively(dto, currentUserId.Value, allComments);
        }
        
        return new SuccessDataResult<List<CommentListDto>>(dto, dto.Count);
    }

    private void LoadRepliesRecursively(Comment comment, List<Comment> allComments)
    {
        var replies = allComments
            .Where(c => c.ParentCommentId == comment.Id)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        comment.Replies = replies;

        // Recursively load replies for each reply
        foreach (var reply in replies)
        {
            LoadRepliesRecursively(reply, allComments);
        }
    }

    private void SetHasLikedRecursively(List<CommentListDto> commentDtos, Guid currentUserId, List<Comment> allComments)
    {
        foreach (var dto in commentDtos)
        {
            SetHasLikedForSingleComment(dto, currentUserId, allComments);
        }
    }
    
    private void SetHasLikedForSingleComment(CommentListDto dto, Guid currentUserId, List<Comment> allComments)
    {
        // Find the original comment by ID
        var original = allComments.FirstOrDefault(c => c.Id == dto.Id);
        if (original != null)
        {
            // Set HasLiked for this comment
            dto.HasLiked = original.Likes.Any(l => l.UserId == currentUserId);
            
            // Recursively set for replies
            if (dto.Replies != null && dto.Replies.Any())
            {
                foreach (var reply in dto.Replies)
                {
                    SetHasLikedForSingleComment(reply, currentUserId, allComments);
                }
            }
        }
    }

    public async Task<DataResult<List<RecentCommentDto>>> GetRecentCommentsAsync(int count)
    {
         var comments = await _unitOfWork.Comments.Query()
            .Include(c => c.User)
            .Include(c => c.Post)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .Select(c => new RecentCommentDto
            {
                Content = c.Content,
                UserName = c.User.UserName,
                CreatedAt = c.CreatedAt,
                Post = new PostInfo
                {
                    Title = c.Post.Title,
                    ImageUrl = c.Post.FeaturedImage,
                    Slug = c.Post.Slug
                }
            })
            .ToListAsync();

        return new SuccessDataResult<List<RecentCommentDto>>(comments, comments.Count);
    }
}
