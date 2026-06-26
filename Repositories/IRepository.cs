using System.Linq.Expressions;

namespace WashNGo.Repositories;

/// <summary>
/// Generic repository interface providing standard CRUD and query operations.
/// </summary>
public interface IRepository<T> where T : class
{
    // ── Queries ───────────────────────────────────────────
    Task<T?>              GetByIdAsync(int id);
    Task<IEnumerable<T>>  GetAllAsync();
    Task<IEnumerable<T>>  FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?>              FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool>            AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int>             CountAsync(Expression<Func<T, bool>>? predicate = null);

    // ── Eager loading ─────────────────────────────────────
    IQueryable<T>         Query();   // for .Include() chaining in specific repos

    // ── Commands ──────────────────────────────────────────
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
