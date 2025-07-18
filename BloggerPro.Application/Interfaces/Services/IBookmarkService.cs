using BloggerPro.Application.DTOs.Bookmark;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface IBookmarkService
    {
        Task<IResult> BookmarkPostAsync(BookmarkCreateDto bookmarkCreateDto);
        Task<IResult> RemoveBookmarkAsync(Guid userId, Guid postId);
        Task<IDataResult<bool>> IsBookmarkedAsync(Guid userId, Guid postId);
        Task<IDataResult<List<BookmarkListDto>>> GetUserBookmarksAsync(Guid userId);
        Task<IDataResult<int>> GetUserBookmarkCountAsync(Guid userId);
        Task<IDataResult<BookmarkDetailDto>> GetBookmarkAsync(Guid userId, Guid postId);
    }
}