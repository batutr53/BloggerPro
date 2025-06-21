using AutoMapper;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class PostModuleProfile : Profile
    {
        public PostModuleProfile()
        {
            CreateMap<PostModuleCreateDto, PostModule>()
    .ForMember(dest => dest.SeoMetadata, opt => opt.MapFrom(src => src.SeoMetadata));

            CreateMap<PostModuleUpdateDto, PostModule>()
                .ForMember(dest => dest.SeoMetadata, opt => opt.MapFrom(src => src.SeoMetadata));

        }
    }
}