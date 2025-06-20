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

    public async Task<DataResult<List<CommentListDto>>> GetCommentsByPostAsync(Guid postId)
    {
        var comments = await _unitOfWork.Comments.Query()
            .Where(c => c.PostId == postId && c.ParentCommentId == null)
            .Include(c => c.User)
            .Include(c => c.Likes)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.Likes)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var dto = _mapper.Map<List<CommentListDto>>(comments);
        return new SuccessDataResult<List<CommentListDto>>(dto, dto.Count);
    }
}
