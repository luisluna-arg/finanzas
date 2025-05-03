using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetLatestCurrencyExchangeRateByShortNameQueryHandler : BaseResponseHandler<GetLatestCurrencyExchangeRateByShortNameQuery, CurrencyExchangeRate?>
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

    public override async Task<CurrencyExchangeRate?> Handle(GetLatestCurrencyExchangeRateByShortNameQuery request, CancellationToken cancellationToken)
    {
        var quoteCurrency = await currencyRepository.GetByAsync("ShortName", request.QuoteCurrencyShortName, cancellationToken);

        if (quoteCurrency == null) return null;

        var result = await currencyExchangeRateRepository.GetByAsync("QuoteCurrencyId", quoteCurrency.Id, cancellationToken);

        return result;
    }
}

public class GetLatestCurrencyExchangeRateByShortNameQuery : IRequest<CurrencyExchangeRate?>
{
    public string QuoteCurrencyShortName { get; set; } = string.Empty;
}
