using AutoMapper;
using BloggerPro.Application.DTOs.Footer;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping;

public class FooterProfile : Profile
{
    public FooterProfile()
    {
        CreateMap<Footer, FooterListDto>().ReverseMap();
        CreateMap<FooterCreateDto, Footer>().ReverseMap();
        CreateMap<FooterUpdateDto, Footer>().ReverseMap();
    }
}