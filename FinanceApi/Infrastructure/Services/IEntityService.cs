using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Infrastructure.Services;

public interface IEntityService<TEntity, TId>
    where TEntity : Entity<TId>
{
    Task<TEntity?> SetDeactivatedAsync(TId id, bool value, CancellationToken cancellationToken);

    Task DeleteAsync(TId id, CancellationToken cancellationToken);

    Task DeleteAsync(ICollection<TId> ids, CancellationToken cancellationToken);
}
