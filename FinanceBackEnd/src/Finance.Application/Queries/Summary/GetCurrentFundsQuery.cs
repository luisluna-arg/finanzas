using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Summary;
using Finance.Domain.Policies;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using FundDto = Finance.Application.Dtos.Summary.FundDto;

namespace Finance.Application.Queries.Summary;

public record GetCurrentFundsQuery : IQuery<TotalFunds>
{
    public bool? DailyUse { get; init; }
    public Guid? CurrencyId { get; init; } = Guid.Parse(CurrencyConstants.PesoId);
}

public class GetCurrentFundsQueryHandler(FinanceDbContext db, ICurrencyConversionPolicy currencyConversionPolicy)
    : IQueryHandler<GetCurrentFundsQuery, TotalFunds>
{
    private readonly FinanceDbContext _db = db;

    public async Task<DataResult<TotalFunds>> ExecuteAsync(GetCurrentFundsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalFunds();

        var fundsQuery = _db.Fund
            .Include(o => o.Bank)
            .Include(o => o.Currency)
                .ThenInclude(o => o != null ? o.Symbols : null)
            .AsSplitQuery() // Split query to avoid Cartesian explosion with multiple includes
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

        var funds = await fundsQuery
            .GroupBy(f => new { f.BankId, f.CurrencyId })
            .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
            .ToListAsync(cancellationToken);

        var currencyExchangeRates = _db.CurrencyExchangeRate
            .Include(o => o.BaseCurrency)
            .Include(o => o.QuoteCurrency)
            .AsSplitQuery() // Split query to avoid Cartesian explosion with multiple includes
            .Where(o => !o.Deactivated);

        var currencyRates = await currencyExchangeRates
            .GroupBy(o => new { o.BaseCurrencyId, o.QuoteCurrencyId })
            .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
            .ToListAsync(cancellationToken);

        Func<Fund, string> nameFormater = (o) => $"{o.Bank!.Name} {o.Currency!.Name}";

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

            var amount = currencyConversionPolicy.Apply(fund.Amount, fund.CurrencyId, currencyRate);

            var fundDto = new FundDto()
            {
                Id = $"{fund.Id}",
                Label = nameFormater(fund),
                Value = fund.Amount,
                BaseCurrencyId = fund.Currency?.Id ?? Guid.Empty,
                BaseCurrency = fund.Currency?.ShortName ?? string.Empty,
                BaseCurrencySymbol = fund.Currency?.Symbols?.FirstOrDefault()?.Symbol ?? string.Empty,
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
