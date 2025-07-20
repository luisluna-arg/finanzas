using Finance.Persistance;
using CQRSDispatch;
using CQRSDispatch.Interfaces;

namespace Finance.Application.Base.Handlers;

public abstract class BaseResponselessHandler<TRequest> : ICommandHandler<TRequest>
    where TRequest : ICommand
{
    protected BaseResponselessHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<CommandResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}
