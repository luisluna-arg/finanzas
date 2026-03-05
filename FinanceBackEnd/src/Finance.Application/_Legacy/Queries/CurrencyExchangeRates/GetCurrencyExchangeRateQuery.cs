using Finance.Application.Legacy.Queries.Base;
using Finance.Application.Legacy.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Legacy.Queries.CurrencyExchangeRates;

public class GetCurrencyExchangeRateQuery : GetSingleByIdQuery<CurrencyExchangeRate, Guid>;

public class GetCurrencyExchangeRateQueryHandler(FinanceDbContext db, IRepository<CurrencyExchangeRate, Guid> currencyRepository)
    : GetSingleByIdQueryHandler<CurrencyExchangeRate, Guid>(db, currencyRepository);
