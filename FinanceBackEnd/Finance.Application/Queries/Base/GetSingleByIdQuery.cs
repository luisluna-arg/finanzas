using Finance.Domain.Models.Interfaces;
using MediatR;

namespace Finance.Application.Queries.Base;

public abstract class GetSingleByIdQuery<TEntity, TId> : IRequest<TEntity>
    where TEntity : IEntity?
{
    public virtual TId Id { get; set; } = default!;
}
