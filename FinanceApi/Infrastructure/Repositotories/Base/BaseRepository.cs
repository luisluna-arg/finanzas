using FinanceApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Infrastructure.Repositotories.Base;

public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
{
    private readonly FinanceDbContext dbContext;
    private readonly DbSet<TEntity> dbSet;

    protected BaseRepository(FinanceDbContext dbContextInstance)
    {
        dbContext = dbContextInstance;
        dbSet = dbContext.Set<TEntity>();
    }

    protected FinanceDbContext DbContext { get; }

    public async Task<TEntity[]> GetAll() => await dbSet.ToArrayAsync();

    public async Task<TEntity> GetById(TId id)
    {
        var result = await dbSet.FindAsync(id);

        if (result == null) throw new ArgumentException($"{nameof(TEntity)} was not found");

        return result;
    }

    public async Task<TEntity> GetBy(string searchCriteria, object searchValue)
    {
        return await GetBy(new Dictionary<string, object>() { { searchCriteria, searchValue } });
    }

    public async Task<TEntity> GetBy(IDictionary<string, object> searchCriteria)
    {
        var result = await GetByQuery(searchCriteria).FirstOrDefaultAsync();

        if (result == null) throw new ArgumentException($"{nameof(TEntity)} was not found");

        return result;
    }

    public IQueryable<TEntity> GetByQuery(IDictionary<string, object> searchCriteria)
    {
        var queryableObjects = dbSet.AsQueryable();

        foreach (var criterion in searchCriteria)
        {
            queryableObjects = queryableObjects.Where(obj =>
                obj.
                    GetType().
                    GetProperty(criterion.Key)!.
                    GetValue(obj)!.
                    Equals(criterion.Value));
        }

        return queryableObjects;
    }

    public async Task Add(TEntity entity, bool persist = true)
    {
        dbSet.Add(entity);
        if (persist) await dbContext.SaveChangesAsync();
    }

    public async Task Update(TEntity entity, bool persist = true)
    {
        dbSet.Attach(entity);
        dbContext.Entry(entity).State = EntityState.Modified;
        if (persist) await dbContext.SaveChangesAsync();
    }

    public async Task Delete(TId entityId, bool persist = true)
        => await Delete(await GetById(entityId), persist);

    public async Task Delete(TEntity entity, bool persist = true)
    {
        dbSet.Remove(entity);
        if (persist) await dbContext.SaveChangesAsync();
    }

    public async Task Persist() => await dbContext.SaveChangesAsync();
}
