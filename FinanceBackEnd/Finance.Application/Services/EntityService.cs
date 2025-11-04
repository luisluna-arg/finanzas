using Finance.Application.Repositories;
using Finance.Domain.Models.Base;

namespace Finance.Application.Services;

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
