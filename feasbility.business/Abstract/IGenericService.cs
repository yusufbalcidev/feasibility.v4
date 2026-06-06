using System.Linq.Expressions;

namespace feasibility.Business.Abstract;

public interface IGenericService<T> where T : class
{
    IQueryable<T> Query(bool ignoreFilters = false);
    Task<List<T>> GetAllAsync(bool ignoreFilters = false, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, bool ignoreFilters = false, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool ignoreFilters = false, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task RestoreAsync(Guid id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
