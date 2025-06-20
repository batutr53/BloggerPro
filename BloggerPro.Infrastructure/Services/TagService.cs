using AutoMapper;
using BloggerPro.Application.DTOs.Tag;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;

namespace BloggerPro.Infrastructure.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TagService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IResult> CreateAsync(TagCreateDto dto)
        {
            var entity = _mapper.Map<Tag>(dto);
            await _uow.Tags.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return new SuccessResult("Etiket başarıyla eklendi.");
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var tag = await _uow.Tags.GetByIdAsync(id);
            if (tag == null)
                return new ErrorResult("Etiket bulunamadı.");

            await _uow.Tags.DeleteAsync(tag); 
            await _uow.SaveChangesAsync();
            return new SuccessResult("Etiket silindi.");
        }

        public async Task<IDataResult<List<TagListDto>>> GetAllAsync()
        {
            var tags = await _uow.Tags.GetAllAsync();
            var mapped = _mapper.Map<List<TagListDto>>(tags);
            return new SuccessDataResult<List<TagListDto>>(mapped);
        }

        public async Task<IDataResult<TagListDto>> GetByIdAsync(Guid id)
        {
            var tag = await _uow.Tags.GetByIdAsync(id);
            if (tag == null)
                return new ErrorDataResult<TagListDto>("Etiket bulunamadı.");

            return new SuccessDataResult<TagListDto>(_mapper.Map<TagListDto>(tag));
        }

        public async Task<IResult> UpdateAsync(TagUpdateDto dto)
        {
            var tag = await _uow.Tags.GetByIdAsync(dto.Id);
            if (tag == null)
                return new ErrorResult("Etiket bulunamadı.");

            _mapper.Map(dto, tag);
            await _uow.Tags.UpdateAsync(tag); 
            await _uow.SaveChangesAsync();
            return new SuccessResult("Etiket güncellendi.");
        }
    }
}
