using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistance;

namespace Finance.Application.Base.Handlers;

public abstract class BaseCollectionHandler<TQuery, TResult> : IQueryHandler<TQuery, List<TResult>>
    where TQuery : IQuery<List<TResult>>
{
    protected BaseCollectionHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<DataResult<List<TResult>>> ExecuteAsync(TQuery command, CancellationToken cancellationToken = default);
}
