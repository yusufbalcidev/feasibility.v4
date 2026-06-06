using System.Linq.Expressions;
using feasibility.Business.Abstract;
using feasibility.DataAccess.Abstract;

namespace feasibility.Business.Concrete;

public class GenericManager<T> : IGenericService<T> where T : class
{
    private readonly IGenericRepository<T> _repository;

    public GenericManager(IGenericRepository<T> repository)
    {
        _repository = repository;
    }

    public IQueryable<T> Query(bool ignoreFilters = false) => _repository.Query(ignoreFilters);

    public Task<List<T>> GetAllAsync(bool ignoreFilters = false, CancellationToken ct = default) =>
        _repository.GetAllAsync(ignoreFilters, ct);

    public Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default) =>
        _repository.GetAllAsync(predicate, ignoreFilters, ct);

    public Task<T?> GetByIdAsync(Guid id, bool ignoreFilters = false, CancellationToken ct = default) =>
        _repository.GetByIdAsync(id, ignoreFilters, ct);

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default) =>
        _repository.FirstOrDefaultAsync(predicate, ignoreFilters, ct);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool ignoreFilters = false, CancellationToken ct = default) =>
        _repository.CountAsync(predicate, ignoreFilters, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        await _repository.UpdateAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _repository.SoftDeleteAsync(id, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task RestoreAsync(Guid id, CancellationToken ct = default)
    {
        await _repository.RestoreAsync(id, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _repository.SaveChangesAsync(ct);
}
