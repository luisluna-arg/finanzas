using Finance.Application.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : class
{
    DbSet<TEntity> GetDbSet();

    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken);

    Task<TEntity[]> GetAllAsync(CancellationToken cancellationToken);

    IQueryable<TEntity> GetAllBy(string searchCriteria, object searchValue);

    IQueryable<TEntity> GetAllBy(IDictionary<string, object> searchCriteria);

    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);

    Task<TEntity?> GetByAsync(string searchCriteria, object searchValue, CancellationToken cancellationToken);

    Task<TEntity?> GetByAsync(IDictionary<string, object> searchCriteria, CancellationToken cancellationToken);

    IQueryable<TEntity> FilterBy(string searchCriteria, ExpressionOperator expressionOperator, object searchValue);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool persist = true);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool persist = true);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool persist = true);

    Task DeleteAsync(TId entityId, CancellationToken cancellationToken, bool persist = true);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool persist = true);

    Task PersistAsync(CancellationToken cancellationToken);
}
