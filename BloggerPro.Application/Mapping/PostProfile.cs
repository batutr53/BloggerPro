using AutoMapper;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostListDto>()
              .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
              .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count));
        CreateMap<Post, PostWithModulesDto>()
    .ForMember(dest => dest.TagIds, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.TagId)))
    .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.Modules));

        CreateMap<PostModule, PostModuleDto>().ReverseMap();

        CreateMap<Post, PostDetailDto>();
        CreateMap<PostCreateDto, Post>();
        CreateMap<PostUpdateDto, Post>().ReverseMap();
    }
}
