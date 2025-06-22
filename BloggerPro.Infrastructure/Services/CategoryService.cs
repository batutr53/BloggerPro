using AutoMapper;
using BloggerPro.Application.DTOs.Category;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Extensions;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IResult> CreateAsync(CategoryCreateDto dto)
        {
            var entity = _mapper.Map<Category>(dto);
            entity.Slug = StringExtensions.GenerateSlug(entity.Name);
            await _uow.Categories.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return new SuccessResult("Kategori oluşturuldu.");
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var entity = await _uow.Categories.GetByIdAsync(id);
            if (entity == null)
                return new ErrorResult("Kategori bulunamadı.");

            await _uow.Categories.DeleteAsync(entity);
            await _uow.SaveChangesAsync();
            return new SuccessResult("Kategori silindi.");
        }

        public async Task<IDataResult<List<CategoryListDto>>> GetAllAsync()
        {
            var list = await _uow.Categories.GetAllAsync();
            var mapped = _mapper.Map<List<CategoryListDto>>(list);
            return new SuccessDataResult<List<CategoryListDto>>(mapped);
        }

        public async Task<IDataResult<CategoryListDto>> GetByIdAsync(Guid id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return new ErrorDataResult<CategoryListDto>("Kategori bulunamadı.");

            return new SuccessDataResult<CategoryListDto>(_mapper.Map<CategoryListDto>(category));
        }

        public async Task<IResult> UpdateAsync(CategoryUpdateDto dto)
        {
            var entity = await _uow.Categories.GetByIdAsync(dto.Id);
            if (entity == null)
                return new ErrorResult("Kategori bulunamadı.");

            _mapper.Map(dto, entity);
            await _uow.Categories.UpdateAsync(entity);
            await _uow.SaveChangesAsync();
            return new SuccessResult("Kategori güncellendi.");
        }
    }
}
