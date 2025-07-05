using Finance.Application.Dtos.Summary;
using MediatR;
using Finance.Persistance;
using Finance.Persistance.Constants;
using Microsoft.EntityFrameworkCore;
using FundDto = Finance.Application.Dtos.Summary.Fund;
using Finance.Domain.Models;

namespace Finance.Application.Queries.Summary;

public class GetCurrentFundsQueryHandler : IRequestHandler<GetCurrentFundsQuery, TotalFunds>
{
    private readonly FinanceDbContext db;

    public GetCurrentFundsQueryHandler(
        FinanceDbContext db)
    {
        this.db = db;
    }

    public async Task<TotalFunds> Handle(GetCurrentFundsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalFunds();

        var fundsQuery = db.Fund
            .Include(o => o.Bank)
            .Include(o => o.Currency)
                .ThenInclude(o => o != null ? o.Symbols : null)
            .Where(o => !o.Deactivated);

        if (request.DailyUse.HasValue)
        {
            fundsQuery = fundsQuery.Where(o => o.DailyUse == request.DailyUse.Value);
        }

        var funds = await fundsQuery
            .GroupBy(o => new { o.BankId, o.CurrencyId })
            .Select(o => o.OrderByDescending(x => x.TimeStamp).First())
            .ToArrayAsync(cancellationToken);

        var currencyRates = await db.CurrencyExchangeRate
            .Where(o => !o.Deactivated)
            .GroupBy(o => new { o.BaseCurrencyId, o.QuoteCurrencyId })
            .Select(o => o.OrderByDescending(x => x.TimeStamp).First())
            .ToArrayAsync(cancellationToken);

        var currencyRates2 = currencyRates
            .GroupBy(o => string.Join("|", new[] { o.BaseCurrencyId, o.QuoteCurrencyId }.OrderBy(x => x)))
            .Select(o => o.OrderByDescending(x => x.TimeStamp).First())
            .ToArray();

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
                    DefaultCurrencyId = currency?.Id ?? Guid.Empty,
                    DefaultCurrency = currency?.ShortName ?? string.Empty,
                    DefaultCurrencySymbol = currencySymbol?.Symbol ?? string.Empty
                };
            }));

        foreach (var fund in funds.Where(o => o.CurrencyId != request.CurrencyId))
        {
            var currencyRate = currencyRates2
                .FirstOrDefault(o => o.BaseCurrencyId == fund.CurrencyId || o.QuoteCurrencyId == fund.CurrencyId);
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
                DefaultCurrencyId = quoteCurrency?.Id ?? Guid.Empty,
                DefaultCurrency = quoteCurrency?.ShortName ?? string.Empty,
                DefaultCurrencySymbol = quoteCurrency?.Symbols?.FirstOrDefault()?.Symbol ?? string.Empty
            };

            result.Add(fundDto);
        }

        return result;
    }
}

public class GetCurrentFundsQuery : IRequest<TotalFunds>
{
    public bool? DailyUse { get; set; }
    public Guid? CurrencyId { get; set; } = Guid.Parse(CurrencyConstants.PesoId);
}
