using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetAllLatestCurrencyExchangeRatesQuery : GetAllQuery<CurrencyExchangeRate>
{
    public GetAllLatestCurrencyExchangeRatesQuery(ICollection<Guid>? currencyIds = default)
        : base()
    {
        CurrencyIds = currencyIds;
    }

    public ICollection<Guid>? CurrencyIds { get; set; }
}

public class GetAllLatestCurrencyExchangeRatesQueryHandler
    : BaseCollectionQueryHandler<GetAllLatestCurrencyExchangeRatesQuery, CurrencyExchangeRate>
{
    public GetAllLatestCurrencyExchangeRatesQueryHandler(FinanceDbContext db) : base(db) { }

    public override async Task<DataResult<List<CurrencyExchangeRate>>> ExecuteAsync(
        GetAllLatestCurrencyExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CurrencyExchangeRate
            .Include(o => o.BaseCurrency)
            .Include(o => o.QuoteCurrency)
            .AsQueryable();

        if (request.CurrencyIds is { Count: > 0 })
            query = query.Where(o => request.CurrencyIds.Contains(o.BaseCurrencyId) || request.CurrencyIds.Contains(o.QuoteCurrencyId));

        if (!request.IncludeDeactivated)
            query = query.Where(o => !o.Deactivated);

        var all = await query.ToListAsync(cancellationToken);
        var result = all
            .GroupBy(o => new { o.BaseCurrencyId, o.QuoteCurrencyId })
            .Select(g => g.MaxBy(o => o.TimeStamp)!)
            .OrderBy(o => o.BaseCurrency.Name)
            .ThenBy(o => o.QuoteCurrency.Name)
            .ToList();

        return DataResult<List<CurrencyExchangeRate>>.Success(result);
    }
}
