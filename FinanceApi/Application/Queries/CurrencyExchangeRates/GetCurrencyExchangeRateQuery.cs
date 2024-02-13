using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.CurrencyExchangeRates;

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
