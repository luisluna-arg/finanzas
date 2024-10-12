using FinanceApi.Domain.Models.Interfaces;
using MediatR;

namespace FinanceApi.Application.Queries.Base;

public abstract class GetSingleByIdQuery<TEntity, TId> : IRequest<TEntity>
    where TEntity : IEntity?
{
    public virtual TId Id { get; set; } = default!;
}
