using Finance.Domain;
using MediatR;
using Finance.Persistance;

namespace Finance.Application.Base.Handlers;

public abstract class BaseResponseHandler<TRequest, TEntity> : IRequestHandler<TRequest, TEntity>
    where TRequest : IRequest<TEntity>
    where TEntity : class?
{
    protected BaseResponseHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<TEntity> Handle(TRequest request, CancellationToken cancellationToken);
}
