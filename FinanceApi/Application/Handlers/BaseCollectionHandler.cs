using FinanceApi.Domain;
using FinanceApi.Domain.Models.Base;
using MediatR;

namespace FinanceApi.Application.Handlers;

public abstract class BaseCollectionHandler<TRequest, TEntity> : IRequestHandler<TRequest, ICollection<TEntity>>
    where TRequest : IRequest<TEntity[]>
    where TEntity : Entity
{
    protected BaseCollectionHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task<ICollection<TEntity>> Handle(TRequest request, CancellationToken cancellationToken);
}
