using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetCurrencyExchangeRateQueryHandler : GetSingleByIdQueryHandler<CurrencyExchangeRate, Guid>
{
    public GetCurrencyExchangeRateQueryHandler(
        FinanceDbContext db,
        IRepository<CurrencyExchangeRate, Guid> currencyRepository)
        : base(db, currencyRepository)
    {
    }
}

public class GetCurrencyExchangeRateQuery : GetSingleByIdQuery<CurrencyExchangeRate?, Guid>
{
}
