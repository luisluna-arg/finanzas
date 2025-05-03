using Finance.Application.Dtos.Summary;
using MediatR;
using Finance.Persistance;
using Finance.Persistance.Constants;
using Microsoft.EntityFrameworkCore;
using FundDto = Finance.Application.Dtos.Summary.Fund;

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
            .Select(o => new FundDto($"{o.Id}", nameFormater(o), o.Amount)));

        foreach (var fund in funds.Where(o => o.CurrencyId != request.CurrencyId))
        {
            var currencyRate = currencyRates2
                .FirstOrDefault(o => o.BaseCurrencyId == fund.CurrencyId || o.QuoteCurrencyId == fund.CurrencyId);
            if (currencyRate == null) continue;

            decimal amount = 0;
            if (fund.CurrencyId == currencyRate.BaseCurrencyId)
            {
                amount = fund.Amount / currencyRate.SellRate;
            }
            else
            {
                amount = fund.Amount * currencyRate.BuyRate;
            }

            result.Add(new FundDto($"{fund.Id}", nameFormater(fund), amount));
        }

        return result;
    }
}

public class GetCurrentFundsQuery : IRequest<TotalFunds>
{
    public bool? DailyUse { get; set; }
    public Guid? CurrencyId { get; set; } = Guid.Parse(CurrencyConstants.PesoId);
}
