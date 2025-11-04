using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetCurrencyExchangeRateQuery : GetSingleByIdQuery<CurrencyExchangeRate, Guid>;

public class GetCurrencyExchangeRateQueryHandler(FinanceDbContext db, IRepository<CurrencyExchangeRate, Guid> currencyRepository)
    : GetSingleByIdQueryHandler<CurrencyExchangeRate, Guid>(db, currencyRepository);
