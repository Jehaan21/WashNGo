using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WashNGo.Data;

namespace WashNGo.Repositories;

/// <summary>
/// Generic EF Core repository implementation.
/// All SaveChanges are deferred to IUnitOfWork.CompleteAsync().
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly WashNGoDbContext _context;
    protected readonly DbSet<T>        _dbSet;

    public Repository(WashNGoDbContext context)
    {
        _context = context;
        _dbSet   = context.Set<T>();
    }

    // ── Queries ───────────────────────────────────────────
    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.FirstOrDefaultAsync(predicate);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AnyAsync(predicate);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate == null
            ? await _dbSet.CountAsync()
            : await _dbSet.CountAsync(predicate);

    public IQueryable<T> Query()
        => _dbSet.AsQueryable();

    // ── Commands ──────────────────────────────────────────
    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Remove(T entity)
        => _dbSet.Remove(entity);
}
