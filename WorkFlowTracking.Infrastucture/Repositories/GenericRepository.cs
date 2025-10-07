using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WorkFlowTracking.Domain.Interfaces;
using WorkFlowTracking.Infrastucture.Data;

namespace WorkFlowTracking.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly WorkflowTrackingDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(WorkflowTrackingDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}