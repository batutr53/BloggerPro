using BloggerPro.Domain.Common;
using System.Linq.Expressions;

namespace BloggerPro.Domain.Repositories;

public interface IGenericRepository<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    void DeleteRange(IEnumerable<T> entities);
    IQueryable<T> Query();
}
