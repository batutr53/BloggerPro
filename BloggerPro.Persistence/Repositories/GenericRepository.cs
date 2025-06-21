using BloggerPro.Domain.Common;
using BloggerPro.Domain.Repositories;
using BloggerPro.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BloggerPro.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id);
    public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
    public async Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        => await _context.Set<T>().Where(predicate).ToListAsync();
    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }
    public Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }
    public void DeleteRange(IEnumerable<T> entities)
    {
        _context.RemoveRange(entities);
    }
    public IQueryable<T> Query() => _context.Set<T>().AsQueryable();
}
