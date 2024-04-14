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

    public async Task<TEntity[]> GetAllAsync(CancellationToken cancellationToken) => await dbSet.ToArrayAsync(cancellationToken);

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken)
        => await dbSet.FindAsync(new Dictionary<string, object>() { { "Id", id! } }, cancellationToken);

    public async Task<TEntity?> GetByAsync(string searchCriteria, object searchValue, CancellationToken cancellationToken)
        => await GetByAsync(new Dictionary<string, object>() { { searchCriteria, searchValue } }, cancellationToken);

    public async Task<TEntity?> GetByAsync(IDictionary<string, object> searchCriteria, CancellationToken cancellationToken)
        => await GetAllBy(searchCriteria).FirstOrDefaultAsync(cancellationToken);

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

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool persist = true)
    {
        dbSet.Add(entity);
        if (persist) await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool persist = true)
    {
        dbSet.AddRange(entities);
        if (persist) await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool persist = true)
    {
        dbSet.Attach(entity);
        dbContext.Entry(entity).State = EntityState.Modified;
        if (persist) await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TId entityId, CancellationToken cancellationToken, bool persist = true)
    {
        var entity = await GetByIdAsync(entityId, cancellationToken);
        if (entity != null)
            await DeleteAsync(entity, cancellationToken, persist);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool persist = true)
    {
        dbSet.Remove(entity);
        if (persist) await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task PersistAsync(CancellationToken cancellationToken) => await dbContext.SaveChangesAsync(cancellationToken);
}
