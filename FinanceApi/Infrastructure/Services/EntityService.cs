using FinanceApi.Domain.Models.Base;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Infrastructure.Services;

public class EntityService<TEntity, TId>
    : IEntityService<TEntity, TId>
    where TEntity : Entity<TId>
{
    private readonly IRepository<TEntity, TId> repository;

    public EntityService(
        IRepository<TEntity, TId> repository)
    {
        this.repository = repository;
    }

    public async Task<TEntity?> SetDeactivated(TId id, bool value)
    {
        var entity = await repository.GetById(id);
        if (entity != null)
        {
            entity.Deactivated = value;
            await repository.Update(entity);
        }

        return entity;
    }
}