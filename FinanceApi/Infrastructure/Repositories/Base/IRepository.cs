namespace FinanceApi.Infrastructure.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : class
{
    Task<TEntity[]> GetAll();

    IQueryable<TEntity> GetAllBy(string searchCriteria, object searchValue);

    IQueryable<TEntity> GetAllBy(IDictionary<string, object> searchCriteria);

    Task<TEntity?> GetById(TId id);

    Task<TEntity?> GetBy(string searchCriteria, object searchValue);

    Task<TEntity?> GetBy(IDictionary<string, object> searchCriteria);

    Task Add(TEntity entity, bool persist = true);

    Task Update(TEntity entity, bool persist = true);

    Task Delete(TId entityId, bool persist = true);

    Task Delete(TEntity entity, bool persist = true);

    Task Persist();
}
