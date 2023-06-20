using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Queries;

public abstract class GetAllQuery<TEntity> : IRequest<TEntity[]>
    where TEntity : IEntity
{
}
