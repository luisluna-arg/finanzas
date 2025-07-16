using CQRSDispatch.Interfaces;
using Finance.Domain.Models.Interfaces;

namespace Finance.Application.Queries.Base;

public abstract class GetAllQuery<TEntity> : IQuery<List<TEntity>>
    where TEntity : IEntity?
{
    public bool IncludeDeactivated { get; set; }
}
