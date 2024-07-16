using FinanceApi.Domain;
using MediatR;

namespace FinanceApi.Application.Base.Handlers;

public abstract class BaseCollectionHandler<TRequest, TEntity> : IRequestHandler<TRequest, ICollection<TEntity>>
    where TRequest : IRequest<ICollection<TEntity>>
    where TEntity : class?
{
    protected BaseCollectionHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<ICollection<TEntity>> Handle(TRequest request, CancellationToken cancellationToken);
}
