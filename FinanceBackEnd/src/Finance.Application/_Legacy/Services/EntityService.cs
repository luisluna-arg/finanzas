using Finance.Application.Legacy.Repositories;
using Finance.Domain.Models.Base;

namespace Finance.Application.Legacy.Services;

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

    public async Task<TEntity?> SetDeactivatedAsync(TId id, bool value, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            entity.Deactivated = value;
            await repository.UpdateAsync(entity, cancellationToken);
        }

        return entity;
    }

    public async Task<ICollection<TEntity>> SetDeactivatedAsync(ICollection<TId> id, bool value, CancellationToken cancellationToken)
    {
        var entities = new List<TEntity>();
        foreach (TId entityId in id)
        {
            var entity = await repository.GetByIdAsync(entityId, cancellationToken);
            if (entity != null)
            {
                entities.Add(entity);
            }
        }
        return entities;
    }

    public async Task DeleteAsync(TId id, CancellationToken cancellationToken)
    {
        await repository.DeleteAsync(id, cancellationToken);
        await repository.PersistAsync(cancellationToken);
    }

    public async Task DeleteAsync(ICollection<TId> ids, CancellationToken cancellationToken)
    {
        foreach (TId id in ids)
        {
            await repository.DeleteAsync(id, cancellationToken);
        }

        await repository.PersistAsync(cancellationToken);
    }
}
