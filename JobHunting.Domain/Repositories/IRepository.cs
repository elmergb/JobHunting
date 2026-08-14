using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace JobHunting.Domain.Repositories
{
    public interface IRepository<T, TId> where T : IBaseEntity<TId> where TId : notnull
    {
        Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        Task DeleteAsync(TId id, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        Task<bool> ExistsAsync(TId id, CancellationToken ct = default);
    }
}
