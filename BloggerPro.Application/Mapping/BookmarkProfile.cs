using AutoMapper;
using BloggerPro.Application.DTOs.Bookmark;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class BookmarkProfile : Profile
    {
        public BookmarkProfile()
        {
            CreateMap<Bookmark, BookmarkListDto>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Post));

            CreateMap<Bookmark, BookmarkDetailDto>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Post));

            CreateMap<BookmarkCreateDto, Bookmark>()
                .ForMember(dest => dest.BookmarkedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore());
        }
    }
}