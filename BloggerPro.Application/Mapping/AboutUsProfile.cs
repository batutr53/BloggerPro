using AutoMapper;
using BloggerPro.Application.DTOs.AboutUs;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping;

public class AboutUsProfile : Profile
{
    public AboutUsProfile()
    {
        CreateMap<AboutUs, AboutUsListDto>();
        CreateMap<AboutUsCreateDto, AboutUs>();
        CreateMap<AboutUsUpdateDto, AboutUs>();
    }
}