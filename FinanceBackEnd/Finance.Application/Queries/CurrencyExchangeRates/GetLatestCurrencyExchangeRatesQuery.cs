using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetLatestCurrencyExchangeRatesQueryHandler : BaseCollectionHandler<GetLatestCurrencyExchangeRatesQuery, CurrencyExchangeRate>
{
    public GetLatestCurrencyExchangeRatesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CurrencyExchangeRate>>> ExecuteAsync(GetLatestCurrencyExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        var latestQuery =
            from o in DbContext.CurrencyExchangeRate
            where (!request.BaseCurrencyId.HasValue || o.BaseCurrencyId == request.BaseCurrencyId.Value)
               && (!request.QuoteCurrencyId.HasValue || o.QuoteCurrencyId == request.QuoteCurrencyId.Value)
               && (request.IncludeDeactivated || !o.Deactivated)
            group o by new { o.BaseCurrencyId, o.QuoteCurrencyId } into g
            select new
            {
                g.Key.BaseCurrencyId,
                g.Key.QuoteCurrencyId,
                LatestTime = g.Max(x => x.TimeStamp)
            };

        var query =
            from o in DbContext.CurrencyExchangeRate
            join l in latestQuery
                on new { o.BaseCurrencyId, o.QuoteCurrencyId, o.TimeStamp }
                equals new { l.BaseCurrencyId, l.QuoteCurrencyId, TimeStamp = l.LatestTime }
            where (!request.BaseCurrencyId.HasValue || o.BaseCurrencyId == request.BaseCurrencyId.Value)
               && (!request.QuoteCurrencyId.HasValue || o.QuoteCurrencyId == request.QuoteCurrencyId.Value)
               && (request.IncludeDeactivated || !o.Deactivated)
            orderby o.BaseCurrency.Name, o.QuoteCurrency.Name
            select o;

        var result = await Task.Run(() => query
            .Include(o => o.BaseCurrency)
                .ThenInclude(c => c.Symbols)
            .Include(o => o.QuoteCurrency)
                .ThenInclude(c => c.Symbols)
            .ToList()
        );

        return DataResult<List<CurrencyExchangeRate>>.Success(result);
    }
}

public class GetLatestCurrencyExchangeRatesQuery : GetAllQuery<CurrencyExchangeRate>
{
    public Guid? BaseCurrencyId { get; set; }

    public Guid? QuoteCurrencyId { get; set; }
}
