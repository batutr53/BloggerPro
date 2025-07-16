using BloggerPro.Application.DTOs.Contact;
using BloggerPro.Application.DTOs.Pagination;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Application.Interfaces.Services;

public interface IContactService
{
    Task<Result> CreateContactAsync(ContactCreateDto contactCreateDto, string? ipAddress = null, string? userAgent = null);
    Task<DataResult<PaginatedResultDto<ContactListDto>>> GetAllContactsAsync(int page = 1, int pageSize = 10, bool? isReplied = null);
    Task<DataResult<ContactListDto>> GetContactByIdAsync(Guid id);
    Task<Result> ReplyToContactAsync(Guid id, ContactReplyDto contactReplyDto);
    Task<Result> DeleteContactAsync(Guid id);
    Task<Result> MarkAsRepliedAsync(Guid id);
}