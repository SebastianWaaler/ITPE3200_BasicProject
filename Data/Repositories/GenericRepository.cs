using Microsoft.EntityFrameworkCore;

namespace QuizMaker.Data.Repositories;

public class GenericRepository<T>(ApplicationDbContext ctx) : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _ctx = ctx;

    public async Task<T?> GetByIdAsync(int id) => await _ctx.Set<T>().FindAsync(id);

    public async Task<List<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> q = _ctx.Set<T>();
        if (filter != null) q = q.Where(filter);
        return await q.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _ctx.Set<T>().AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _ctx.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _ctx.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveAsync() => await _ctx.SaveChangesAsync();
}
