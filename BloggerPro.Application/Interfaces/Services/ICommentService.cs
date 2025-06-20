using BloggerPro.Application.DTOs.Comment;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface ICommentService
    {
        Task<DataResult<Guid>> AddCommentAsync(CommentCreateDto dto, Guid userId);
        Task<Result> DeleteCommentAsync(Guid commentId, Guid userId);
        Task<DataResult<List<CommentListDto>>> GetCommentsByPostAsync(Guid postId);
    }

}
