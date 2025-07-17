using AutoMapper;
using BloggerPro.Application.DTOs.Contact;
using BloggerPro.Application.DTOs.Pagination;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services;

public class ContactService : IContactService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContactService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> CreateContactAsync(ContactCreateDto contactCreateDto, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var contact = _mapper.Map<Contact>(contactCreateDto);
            contact.IpAddress = ipAddress;
            contact.UserAgent = userAgent;
            contact.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Contacts.AddAsync(contact);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("İletişim mesajınız başarıyla gönderildi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"İletişim mesajı gönderilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<DataResult<PaginatedResultDto<ContactListDto>>> GetAllContactsAsync(int page = 1, int pageSize = 10, bool? isReplied = null)
    {
        try
        {
            var query = _unitOfWork.Contacts.Query()
                .Where(c => !c.IsDeleted); // IsDeleted = false olan kayıtları al

            if (isReplied.HasValue)
            {
                query = query.Where(c => c.IsReplied == isReplied.Value);
            }

            var totalCount = await query.CountAsync();
            var contacts = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var contactsDto = _mapper.Map<List<ContactListDto>>(contacts);

            var result = new PaginatedResultDto<ContactListDto>(contactsDto, totalCount, page, pageSize);

            return new SuccessDataResult<PaginatedResultDto<ContactListDto>>(result);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<PaginatedResultDto<ContactListDto>>($"İletişim mesajları listelenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<DataResult<ContactListDto>> GetContactByIdAsync(Guid id)
    {
        try
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            if (contact == null || contact.IsDeleted)
            {
                return new ErrorDataResult<ContactListDto>("İletişim mesajı bulunamadı.");
            }

            var contactDto = _mapper.Map<ContactListDto>(contact);
            return new SuccessDataResult<ContactListDto>(contactDto);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<ContactListDto>($"İletişim mesajı getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> ReplyToContactAsync(Guid id, ContactReplyDto contactReplyDto)
    {
        try
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            if (contact == null || contact.IsDeleted)
            {
                return new ErrorResult("İletişim mesajı bulunamadı.");
            }

            contact.AdminReply = contactReplyDto.AdminReply;
            contact.IsReplied = true;
            contact.RepliedAt = DateTime.UtcNow;
            contact.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Contacts.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Yanıt başarıyla gönderildi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Yanıt gönderilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> DeleteContactAsync(Guid id)
    {
        try
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            if (contact == null || contact.IsDeleted)
            {
                return new ErrorResult("İletişim mesajı bulunamadı.");
            }

            contact.IsDeleted = true;
            contact.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Contacts.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("İletişim mesajı başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"İletişim mesajı silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> MarkAsRepliedAsync(Guid id)
    {
        try
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            if (contact == null || contact.IsDeleted)
            {
                return new ErrorResult("İletişim mesajı bulunamadı.");
            }

            contact.IsReplied = true;
            contact.RepliedAt = DateTime.UtcNow;
            contact.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Contacts.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Mesaj yanıtlandı olarak işaretlendi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Mesaj işaretlenirken hata oluştu: {ex.Message}");
        }
    }
}