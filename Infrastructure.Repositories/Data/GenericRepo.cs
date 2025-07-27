using Infrastructure.Interfaces.Lead;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Data;

public class GenericRepo<T>(LeadContext context) : IGenericRepo<T> where T : class
{
    private readonly LeadContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<IList<T>> GetAllAsync(dynamic? parameter = null)
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IList<T>> GetAllWithTracingAsync(dynamic? parameter = null)
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IList<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveRangeAsync(IList<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
}


