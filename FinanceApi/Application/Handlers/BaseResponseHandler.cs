using FinanceApi.Domain;
using MediatR;

namespace FinanceApi.Application.Handlers;

public abstract class BaseResponseHandler<TRequest, TEntity> : IRequestHandler<TRequest, TEntity>
    where TRequest : IRequest<TEntity>
    where TEntity : class
{
    protected BaseResponseHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<TEntity> Handle(TRequest request, CancellationToken cancellationToken);
}
