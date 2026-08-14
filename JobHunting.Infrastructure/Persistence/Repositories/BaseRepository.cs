using JobHunting.Domain.Primatives;
using JobHunting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace JobHunting.Infrastructure.Persistence.Repositories
{
    public class BaseRepository<T, TId> : IRepository<T, TId> 
        where T : class, IBaseEntity<TId> 
        where TId : notnull
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(TId id, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id), ct);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync(ct);
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(predicate, ct);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync(ct);
        }
        public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }
        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _dbSet.AddRangeAsync(entities, ct);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task DeleteAsync(TId id, CancellationToken ct = default)
        {
            var entity = await GetByIdAsync(id, ct);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task<bool> ExistsAsync(TId id, CancellationToken ct = default)
        {
            return await AnyAsync(e => e.Id.Equals(id), ct);
        }
    }
}
