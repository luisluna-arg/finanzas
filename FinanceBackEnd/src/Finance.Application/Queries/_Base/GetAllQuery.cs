using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Models.Interfaces;

namespace Finance.Application.Queries.Base;

public abstract class GetAllQuery<TEntity> : IContextAwareQuery<FinanceDispatchContext, List<TEntity>>
    where TEntity : IEntity?
{
    public bool IncludeDeactivated { get; set; }
    internal FinanceDispatchContext Context { get; private set; } = new();

    public void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}
