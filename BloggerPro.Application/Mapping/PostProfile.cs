using AutoMapper;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Application.DTOs.SeoMetadata;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.Mapping;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostModule, PostModuleDto>().ReverseMap();
        // Post to DTO mappings
        CreateMap<Post, PostListDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
             .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.PostCategories.Select(pc => pc.Category.Name)))
    .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name)))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.Ratings.Any() ? src.Ratings.Average(r => r.RatingValue) : 0.0));

        CreateMap<Post, PostDetailDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.Author.ProfileImageUrl))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.PostCategories.Select(pc => pc.Category.Name)))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name)))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.Ratings.Any() ? src.Ratings.Average(r => r.RatingValue) : 0.0));

        // DTO to Post mappings
        CreateMap<CreatePostDto, Post>()
            .ForMember(dest => dest.PostCategories, opt => opt.Ignore())
            .ForMember(dest => dest.PostTags, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PostStatus.Draft))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<PostCreateDto, Post>()
.ForMember(dest => dest.PostCategories, opt => opt.Ignore())
.ForMember(dest => dest.PostTags, opt => opt.Ignore())
.ForMember(dest => dest.Modules, opt => opt.Ignore())
.ForMember(dest => dest.Status, opt => opt.MapFrom(src => PostStatus.Draft))
.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdatePostDto, Post>()
            .ForMember(dest => dest.PostCategories, opt => opt.Ignore())
            .ForMember(dest => dest.PostTags, opt => opt.Ignore())
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<PostUpdateDto, Post>()
    .ForMember(dest => dest.PostCategories, opt => opt.Ignore())
    .ForMember(dest => dest.PostTags, opt => opt.Ignore())
    .ForMember(dest => dest.Modules, opt => opt.Ignore())
    .ForMember(dest => dest.LastModified, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // PostModule mappings
        CreateMap<PostModule, PostModuleDto>().ReverseMap();
        CreateMap<CreatePostModuleDto, PostModule>();
        CreateMap<UpdatePostModuleDto, PostModule>();

        // Status and visibility DTOs
        CreateMap<UpdatePostStatusDto, Post>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src =>
                src.Status == PostStatus.Published && !src.PublishDate.HasValue ? DateTime.UtcNow : src.PublishDate));

        CreateMap<UpdatePostVisibilityDto, Post>()
            .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility));

        CreateMap<SeoMetadata, SeoMetadataDto>();
    }
}