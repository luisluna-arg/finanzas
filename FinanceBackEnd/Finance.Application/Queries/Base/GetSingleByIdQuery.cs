using CQRSDispatch.Interfaces;
using Finance.Domain.Models.Interfaces;

namespace Finance.Application.Queries.Base;

public abstract class GetSingleByIdQuery<TEntity, TId> : IQuery<TEntity?>
    where TEntity : IEntity?
{
    public virtual TId Id { get; set; } = default!;
}
