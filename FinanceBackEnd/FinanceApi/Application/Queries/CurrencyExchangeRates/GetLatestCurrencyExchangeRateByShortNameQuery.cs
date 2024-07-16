using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Queries.CurrencyExchangeRates;

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
    public string QuoteCurrencyShortName { get; set; }
}
