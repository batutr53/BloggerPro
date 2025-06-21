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
    
    /// <summary>
    /// Finds entities based on a condition
    /// </summary>
    /// <param name="predicate">The condition to filter entities</param>
    /// <param name="trackChanges">Whether to track changes (default: true)</param>
    /// <returns>An IQueryable of entities that match the condition</returns>
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> predicate, bool trackChanges = true);
    
    /// <summary>
    /// Returns the number of elements in a sequence
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The number of elements in the sequence</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns the number of elements in a sequence that satisfy a condition
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The number of elements in the sequence that satisfy the condition</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Determines whether any element of a sequence exists
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>true if any elements in the source sequence pass the test in the specified predicate; otherwise, false</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

}
