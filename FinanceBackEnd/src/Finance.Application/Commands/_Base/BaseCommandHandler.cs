using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistence;

namespace Finance.Application.Base.Handlers;

public abstract class BaseCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, DataResult<TResult>>
    where TCommand : ICommand
    where TResult : class
{
    protected BaseCommandHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<DataResult<TResult>> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
}
