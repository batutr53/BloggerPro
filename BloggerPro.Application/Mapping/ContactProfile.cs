using AutoMapper;
using BloggerPro.Application.DTOs.Contact;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping;

public class ContactProfile : Profile
{
    public ContactProfile()
    {
        CreateMap<ContactCreateDto, Contact>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsReplied, opt => opt.Ignore())
            .ForMember(dest => dest.RepliedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AdminReply, opt => opt.Ignore())
            .ForMember(dest => dest.IpAddress, opt => opt.Ignore())
            .ForMember(dest => dest.UserAgent, opt => opt.Ignore());

        CreateMap<Contact, ContactListDto>();
    }
}