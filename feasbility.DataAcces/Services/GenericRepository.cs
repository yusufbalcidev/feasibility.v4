using System.Linq.Expressions;
using System.Reflection;
using feasibility.DataAccess.Abstract;
using feasibility.DataAccess.Context;
using feasibility.Entity.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace feasibility.DataAccess.Services;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _set;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public IQueryable<T> Query(bool ignoreFilters = false) =>
        ignoreFilters ? _set.IgnoreQueryFilters() : _set;

    public async Task<List<T>> GetAllAsync(bool ignoreFilters = false, CancellationToken ct = default) =>
        await Query(ignoreFilters).ToListAsync(ct);

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default) =>
        await Query(ignoreFilters).Where(predicate).ToListAsync(ct);

    public async Task<T?> GetByIdAsync(Guid id, bool ignoreFilters = false, CancellationToken ct = default)
    {
        var query = Query(ignoreFilters);
        return await query.FirstOrDefaultAsync(BuildIdPredicate(id), ct);
    }

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default) =>
        Query(ignoreFilters).FirstOrDefaultAsync(predicate, ct);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool ignoreFilters = false, CancellationToken ct = default) =>
        predicate is null
            ? Query(ignoreFilters).CountAsync(ct)
            : Query(ignoreFilters).Where(predicate).CountAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _set.IgnoreQueryFilters().FirstOrDefaultAsync(BuildIdPredicate(id), ct);
        if (entity is null) return;

        if (entity is BaseEntity baseEntity)
        {
            baseEntity.IsDeleted = true;
            _set.Update(entity);
            return;
        }

        var isDeletedProp = typeof(T).GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance);
        if (isDeletedProp is not null && isDeletedProp.PropertyType == typeof(bool))
        {
            isDeletedProp.SetValue(entity, true);
            _set.Update(entity);
            return;
        }

        _set.Remove(entity);
    }

    public async Task RestoreAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _set.IgnoreQueryFilters().FirstOrDefaultAsync(BuildIdPredicate(id), ct);
        if (entity is not BaseEntity baseEntity || !baseEntity.IsDeleted) return;

        baseEntity.IsDeleted      = false;
        baseEntity.DeletedAt      = null;
        baseEntity.DeletedBy      = null;
        baseEntity.DeletedByName  = null;
        _set.Update(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);

    private static Expression<Func<T, bool>> BuildIdPredicate(Guid id)
    {
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, "Id");
        var constant = Expression.Constant(id);
        var equal = Expression.Equal(property, constant);
        return Expression.Lambda<Func<T, bool>>(equal, parameter);
    }
}
