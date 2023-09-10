using FinanceApi.Domain;
using MediatR;

namespace FinanceApi.Application.Base.Handlers;

public abstract class BaseResponselessHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest
{
    protected BaseResponselessHandler(FinanceDbContext db)
    {
        DbContext = db;
    }

    protected FinanceDbContext DbContext { get; private set; }

    public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}
