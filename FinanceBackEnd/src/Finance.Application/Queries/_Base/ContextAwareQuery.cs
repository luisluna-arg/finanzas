using CQRSDispatch.Interfaces;
using Finance.Application.Auth;

namespace Finance.Application.Queries.Base;

public class ContextAwareQuery<TResult> : IContextAwareQuery<FinanceDispatchContext, TResult>
{
    public FinanceDispatchContext? _context;

    public void SetContext(FinanceDispatchContext context)
    {
        _context = context;
    }

    public FinanceDispatchContext? Context { get => _context; }
}
