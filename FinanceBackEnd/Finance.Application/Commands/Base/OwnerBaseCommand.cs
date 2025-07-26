using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;

namespace Finance.Application.Commands.Users;

public abstract class OwnerBaseCommand<TResult> : IContextAwareCommand<FinanceDispatchContext, TResult>
    where TResult : RequestResult
{
    public FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}