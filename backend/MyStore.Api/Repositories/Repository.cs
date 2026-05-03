using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyStore.Api.Data;

namespace MyStore.Api.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<T> Set;

    public Repository(AppDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public IQueryable<T> Query() => Set.AsQueryable();

    public Task<T?> GetByIdAsync(int id) => Set.FindAsync(id).AsTask();

    public Task<List<T>> ListAsync() => Set.ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await Set.AddAsync(entity);
        return entity;
    }

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public Task<int> SaveChangesAsync() => Db.SaveChangesAsync();

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => Set.AnyAsync(predicate);
}
