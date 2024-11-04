using Finance.Domain.Models.Interfaces;
using MediatR;

namespace Finance.Application.Queries.Base;

public abstract class GetAllQuery<TEntity> : IRequest<ICollection<TEntity>>
    where TEntity : IEntity?
{
    public bool IncludeDeactivated { get; set; }
}
