using BloggerPro.Domain.Common;
using BloggerPro.Domain.Repositories;
using BloggerPro.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BloggerPro.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    protected readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
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
        var entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _context.Attach(entity);
            entry.State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
    public Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }
    public void DeleteRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }
    public IQueryable<T> Query() => _context.Set<T>().AsQueryable();
    
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> predicate, bool trackChanges = true)
    {
        var query = _context.Set<T>().Where(predicate);
        
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }
        
        return query;
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().AnyAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().AnyAsync(predicate, cancellationToken);
    }
}
