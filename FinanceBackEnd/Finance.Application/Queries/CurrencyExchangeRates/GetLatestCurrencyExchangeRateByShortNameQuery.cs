using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetLatestCurrencyExchangeRateByShortNameQueryHandler : BaseQueryHandler<GetLatestCurrencyExchangeRateByShortNameQuery, CurrencyExchangeRate?>
{
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository;

    public GetLatestCurrencyExchangeRateByShortNameQueryHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
        this.currencyExchangeRateRepository = currencyExchangeRateRepository;
    }

    public override async Task<DataResult<CurrencyExchangeRate?>> ExecuteAsync(GetLatestCurrencyExchangeRateByShortNameQuery request, CancellationToken cancellationToken)
    {
        var quoteCurrency = await currencyRepository.GetByAsync("ShortName", request.QuoteCurrencyShortName, cancellationToken);

        if (quoteCurrency == null) return DataResult<CurrencyExchangeRate?>.Failure($"Currency with short name '{request.QuoteCurrencyShortName}' not found.");

        var result = await currencyExchangeRateRepository.GetByAsync("QuoteCurrencyId", quoteCurrency.Id, cancellationToken);

        return DataResult<CurrencyExchangeRate?>.Success(result);
    }
}

public class GetLatestCurrencyExchangeRateByShortNameQuery : IQuery<CurrencyExchangeRate?>
{
    public string QuoteCurrencyShortName { get; set; } = string.Empty;
}
