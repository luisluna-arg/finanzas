using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Base;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetSingleByIdQueryHandler<TEntity, TId> : BaseResponseHandler<GetSingleByIdQuery<TEntity?, TId>, TEntity?>
    where TEntity : Entity<TId>
{
    protected readonly IRepository<TEntity, TId> repository;

    public GetSingleByIdQueryHandler(
        FinanceDbContext db,
        IRepository<TEntity, TId> repository)
        : base(db)
    {
        this.repository = repository;
    }

    public override async Task<TEntity?> Handle(GetSingleByIdQuery<TEntity?, TId> request, CancellationToken cancellationToken)
        => await repository.GetByIdAsync(request.Id, cancellationToken);
}