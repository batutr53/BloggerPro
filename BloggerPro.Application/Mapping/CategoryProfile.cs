using AutoMapper;
using BloggerPro.Application.DTOs.Category;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryListDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>().ReverseMap();
        }
    }

}
