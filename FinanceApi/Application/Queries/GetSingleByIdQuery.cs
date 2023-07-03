using FinanceApi.Domain.Models.Interfaces;
using MediatR;

namespace FinanceApi.Application.Queries;

public abstract class GetSingleByIdQuery<TEntity, TId> : IRequest<TEntity>
    where TEntity : IEntity
{
    public TId Id { get; set; } = default!;
}
