using AutoMapper;
using BloggerPro.Application.DTOs.Tag;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagListDto>();
            CreateMap<TagCreateDto, Tag>();
            CreateMap<TagUpdateDto, Tag>().ReverseMap();
        }
    }

}
