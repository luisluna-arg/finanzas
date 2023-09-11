using System.Linq.Expressions;
using FinanceApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Infrastructure.Repositories.Base;

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

    protected FinanceDbContext DbContext { get => dbContext; }

    public DbSet<TEntity> GetDbSet() => dbSet;

    public async Task<TEntity[]> GetAll() => await dbSet.ToArrayAsync();

    public async Task<TEntity?> GetById(TId id) => await dbSet.FindAsync(id);

    public async Task<TEntity?> GetBy(string searchCriteria, object searchValue)
        => await GetBy(new Dictionary<string, object>() { { searchCriteria, searchValue } });

    public async Task<TEntity?> GetBy(IDictionary<string, object> searchCriteria)
        => await GetAllBy(searchCriteria).FirstOrDefaultAsync();

    public IQueryable<TEntity> GetAllBy(string searchCriteria, object searchValue)
        => GetAllBy(new Dictionary<string, object>() { { searchCriteria, searchValue } });

    public IQueryable<TEntity> GetAllBy(IDictionary<string, object> searchCriteria)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression body = Expression.Constant(true);

        foreach (var filter in searchCriteria)
        {
            var property = Expression.Property(parameter, filter.Key);
            var value = Expression.Constant(filter.Value);
            var equality = Expression.Equal(property, value);
            body = Expression.AndAlso(body, equality);
        }

        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return dbSet.Where(lambda);
    }

    public IQueryable<TEntity> FilterBy(string searchCriteria, ExpressionOperator expressionOperator, object searchValue)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, searchCriteria);
        var value = Expression.Constant(searchValue);

        Expression operation;
        switch (expressionOperator)
        {
            case ExpressionOperator.GreaterThan:
                operation = Expression.GreaterThan(property, value);
                break;
            case ExpressionOperator.GreaterThanOrEqual:
                operation = Expression.GreaterThanOrEqual(property, value);
                break;
            case ExpressionOperator.LessThan:
                operation = Expression.LessThan(property, value);
                break;
            case ExpressionOperator.LessThanOrEqual:
                operation = Expression.LessThanOrEqual(property, value);
                break;
            default:
                operation = Expression.Equal(property, value);
                break;
        }

        return dbSet.Where(Expression.Lambda<Func<TEntity, bool>>(operation, parameter));
    }

    public async Task Add(TEntity entity, bool persist = true)
    {
        dbSet.Add(entity);
        if (persist) await dbContext.SaveChangesAsync();
    }

    public async Task AddRange(IEnumerable<TEntity> entities, bool persist = true)
    {
        dbSet.AddRange(entities);
        if (persist) await dbContext.SaveChangesAsync();
    }

    public async Task Update(TEntity entity, bool persist = true)
    {
        dbSet.Attach(entity);
        dbContext.Entry(entity).State = EntityState.Modified;
        if (persist) await dbContext.SaveChangesAsync();
    }

    public async Task Delete(TId entityId, bool persist = true)
    {
        var entity = await GetById(entityId);
        if (entity != null)
            await Delete(entity, persist);
    }

    public async Task Delete(TEntity entity, bool persist = true)
    {
        dbSet.Remove(entity);
        if (persist) await dbContext.SaveChangesAsync();
    }

    public async Task Persist() => await dbContext.SaveChangesAsync();
}
