using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.Currencies;

public class GetAllCurrenciesQueryHandler : BaseCollectionHandler<GetAllCurrenciesQuery, Currency>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public GetAllCurrenciesQueryHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<ICollection<Currency>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
        => await currencyRepository.GetAll();
}

public class GetAllCurrenciesQuery : GetAllQuery<Currency>
{
}
