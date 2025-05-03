using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetLatestCurrencyExchangeRatesQueryHandler : BaseCollectionHandler<GetLatestCurrencyExchangeRatesQuery, CurrencyExchangeRate?>
{
    public GetLatestCurrencyExchangeRatesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CurrencyExchangeRate?>> Handle(GetLatestCurrencyExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CurrencyExchangeRate
            .Include(o => o.BaseCurrency)
            .Include(o => o.QuoteCurrency)
            .AsQueryable();

        if (request.BaseCurrencyId.HasValue)
        {
            query = query.Where(o => o.BaseCurrencyId == request.BaseCurrencyId.Value);
        }

        if (request.QuoteCurrencyId.HasValue)
        {
            query = query.Where(o => o.QuoteCurrencyId == request.QuoteCurrencyId.Value);
        }

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        var partialResult = await query
            .OrderBy(o => o.BaseCurrency.Name)
            .ThenBy(o => o.QuoteCurrency.Name)
            .ThenByDescending(o => o.TimeStamp)
            .ToArrayAsync();

        var groupResult = partialResult.GroupBy(
            child => new { child.BaseCurrencyId, child.QuoteCurrencyId },
            (key, group) => group);

        return await Task.FromResult(groupResult.Select(o => o.First()).ToArray());
    }
}

public class GetLatestCurrencyExchangeRatesQuery : GetAllQuery<CurrencyExchangeRate?>
{
    public Guid? BaseCurrencyId { get; set; }

    public Guid? QuoteCurrencyId { get; set; }
}
