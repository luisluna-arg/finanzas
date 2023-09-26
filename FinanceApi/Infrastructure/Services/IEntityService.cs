using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Infrastructure.Services;

public interface IEntityService<TEntity, TId>
    where TEntity : Entity<TId>
{
    Task<TEntity?> SetDeactivated(TId id, bool value);
}
