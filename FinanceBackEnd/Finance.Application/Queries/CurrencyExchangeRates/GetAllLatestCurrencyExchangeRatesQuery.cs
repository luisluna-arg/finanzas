using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetAllLatestCurrencyExchangeRatesQueryHandler
    : BaseCollectionHandler<GetAllLatestCurrencyExchangeRatesQuery, CurrencyExchangeRate>
{
    public GetAllLatestCurrencyExchangeRatesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CurrencyExchangeRate>>> ExecuteAsync(
        GetAllLatestCurrencyExchangeRatesQuery request,
        CancellationToken cancellationToken)
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

        var groupResult = query.GroupBy(
            child => new { child.BaseCurrencyId, child.QuoteCurrencyId },
            (key, group) =>
                group
                    .OrderBy(o => o.BaseCurrency.Name)
                    .ThenBy(o => o.QuoteCurrency.Name)
                    .ThenByDescending(o => o.TimeStamp)
                    .AsEnumerable());

        return DataResult<List<CurrencyExchangeRate>>.Success(await groupResult.Select(o => o.First()).ToListAsync(cancellationToken));
    }
}

public class GetAllLatestCurrencyExchangeRatesQuery : GetAllQuery<CurrencyExchangeRate>
{
    public GetAllLatestCurrencyExchangeRatesQuery(
        Guid? baseCurrencyId = default,
        Guid? quoteCurrencyId = default)
        : base()
    {
        BaseCurrencyId = baseCurrencyId;
        QuoteCurrencyId = quoteCurrencyId;
    }

    public Guid? BaseCurrencyId { get; set; }
    public Guid? QuoteCurrencyId { get; set; }
}
