using FinanceApi.Domain.Models.Interfaces;
using MediatR;

namespace FinanceApi.Application.Queries.Base;

public abstract class GetAllQuery<TEntity> : IRequest<ICollection<TEntity>>
    where TEntity : IEntity
{
}
