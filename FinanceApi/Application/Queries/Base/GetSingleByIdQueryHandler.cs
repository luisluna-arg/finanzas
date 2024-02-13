using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models.Base;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.CurrencyExchangeRates;

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
        => await repository.GetById(request.Id);
}