using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Queries;

public abstract class GetSingleByIdQuery<TEntity> : IRequest<TEntity>
    where TEntity : IEntity
{
    public Guid Id { get; set; }
}
