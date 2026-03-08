using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistence;

namespace Finance.Application.Commands.Base;

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
