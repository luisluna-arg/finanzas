using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistence;

namespace Finance.Application.Base.Handlers;

public abstract class BaseQueryHandler<TQuery, TEntity> : IQueryHandler<TQuery, TEntity>
    where TQuery : IQuery<TEntity>
{
    protected BaseQueryHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<DataResult<TEntity>> ExecuteAsync(TQuery request, CancellationToken cancellationToken);
}
