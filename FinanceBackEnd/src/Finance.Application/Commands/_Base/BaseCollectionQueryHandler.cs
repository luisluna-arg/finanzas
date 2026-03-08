using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistence;

namespace Finance.Application.Base.Handlers;

public abstract class BaseCollectionQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, List<TResult>>
    where TQuery : IQuery<List<TResult>>
{
    protected BaseCollectionQueryHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<DataResult<List<TResult>>> ExecuteAsync(TQuery command, CancellationToken cancellationToken = default);
}
