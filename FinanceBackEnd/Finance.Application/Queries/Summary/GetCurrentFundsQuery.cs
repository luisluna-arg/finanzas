using Finance.Application.Dtos.Summary;
using Finance.Persistence;
using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using FundDto = Finance.Application.Dtos.Summary.Fund;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;

namespace Finance.Application.Queries.Summary;

public class GetCurrentFundsQueryHandler : IQueryHandler<GetCurrentFundsQuery, TotalFunds>
{
    private readonly FinanceDbContext _db;

    public GetCurrentFundsQueryHandler(
        FinanceDbContext db)
    {
        _db = db;
    }

    public async Task<DataResult<TotalFunds>> ExecuteAsync(GetCurrentFundsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalFunds();

        var fundsQuery = _db.Fund
            .Include(o => o.Bank)
            .Include(o => o.Currency)
                .ThenInclude(o => o != null ? o.Symbols : null)
            .Where(o => !o.Deactivated);

        if (request.DailyUse.HasValue)
        {
            fundsQuery = fundsQuery.Where(o => o.DailyUse == request.DailyUse.Value);
        }

        var defaultCurrency = await _db.Currency
            .FirstOrDefaultAsync(o => o.Id == Guid.Parse(CurrencyConstants.DefaultCurrencyId), cancellationToken);

        if (defaultCurrency == null)
        {
            return DataResult<TotalFunds>.Failure("Default currency not found.");
        }

        var bankIds = await fundsQuery.Select(o => o.BankId).Distinct().ToArrayAsync(cancellationToken);

        var funds = new List<Domain.Models.Fund>();

        foreach (var bankId in bankIds)
        {
            var latestFunds = await fundsQuery
                .Where(f => f.BankId == bankId)
                .GroupBy(f => f.CurrencyId)
                .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
                .ToListAsync(cancellationToken);
            funds.AddRange(latestFunds);
        }

        var currencyExchangeRates = _db.CurrencyExchangeRate
            .Include(o => o.BaseCurrency)
            .Include(o => o.QuoteCurrency)
            .Where(o => !o.Deactivated);

        var baseCurrencyIds = await currencyExchangeRates
            .Select(o => o.BaseCurrencyId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        var currencyRates = new List<CurrencyExchangeRate>();
        foreach (var baseCurrencyId in baseCurrencyIds)
        {
            var latestRates = await currencyExchangeRates
                .Where(o => o.BaseCurrencyId == baseCurrencyId)
                .GroupBy(o => o.QuoteCurrencyId)
                .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
                .ToListAsync(cancellationToken);
            currencyRates.AddRange(latestRates);
        }

        Func<Domain.Models.Fund, string> nameFormater = (o) => $"{o.Bank!.Name} {o.Currency!.Name}";

        result.Items.AddRange(funds
            .Where(o => o.CurrencyId == request.CurrencyId)
            .Select(o =>
            {
                var currency = o.Currency;
                var currencySymbol = currency?.Symbols.FirstOrDefault();

                return new FundDto()
                {
                    Id = $"{o.Id}",
                    Label = nameFormater(o),
                    Value = o.Amount,
                    BaseCurrencyId = currency?.Id ?? Guid.Empty,
                    BaseCurrency = currency?.ShortName ?? string.Empty,
                    BaseCurrencySymbol = currencySymbol?.Symbol ?? string.Empty,
                    QuoteCurrencyValue = o.Amount,
                    DefaultCurrencyId = defaultCurrency?.Id ?? Guid.Empty,
                    DefaultCurrency = defaultCurrency?.ShortName ?? string.Empty,
                    DefaultCurrencySymbol = defaultCurrency?.Symbols.FirstOrDefault()?.Symbol ?? string.Empty
                };
            }));

        foreach (var fund in funds.Where(o => o.CurrencyId != request.CurrencyId))
        {
            var currencyRate = currencyRates
                .FirstOrDefault(o => (o.BaseCurrencyId == defaultCurrency!.Id && o.QuoteCurrencyId == fund.CurrencyId) ||
                    (o.BaseCurrencyId == fund.CurrencyId && o.QuoteCurrencyId == defaultCurrency!.Id));
            if (currencyRate == null) continue;

            Currency baseCurrency, quoteCurrency;

            decimal amount = 0;
            if (fund.CurrencyId == currencyRate.BaseCurrencyId)
            {
                baseCurrency = currencyRate.BaseCurrency;
                quoteCurrency = currencyRate.QuoteCurrency;

                amount = fund.Amount / currencyRate.SellRate;
            }
            else
            {
                baseCurrency = currencyRate.QuoteCurrency;
                quoteCurrency = currencyRate.BaseCurrency;

                amount = fund.Amount * currencyRate.BuyRate;
            }

            var fundDto = new FundDto()
            {
                Id = $"{fund.Id}",
                Label = nameFormater(fund),
                Value = fund.Amount,
                BaseCurrencyId = baseCurrency?.Id ?? Guid.Empty,
                BaseCurrency = baseCurrency?.ShortName ?? string.Empty,
                BaseCurrencySymbol = baseCurrency?.Symbols?.FirstOrDefault()?.Symbol ?? string.Empty,
                QuoteCurrencyValue = amount,
                DefaultCurrencyId = defaultCurrency?.Id ?? Guid.Empty,
                DefaultCurrency = defaultCurrency?.ShortName ?? string.Empty,
                DefaultCurrencySymbol = defaultCurrency?.Symbols.FirstOrDefault()?.Symbol ?? string.Empty
            };

            result.Add(fundDto);
        }

        return DataResult<TotalFunds>.Success(result);
    }
}

public class GetCurrentFundsQuery : IQuery<TotalFunds>
{
    public bool? DailyUse { get; set; }
    public Guid? CurrencyId { get; set; } = Guid.Parse(CurrencyConstants.PesoId);
}
