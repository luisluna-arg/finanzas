using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CurrencyExchangeRates;

public class GetAllCurrencyExchangeRatesQueryHandler : BaseCollectionHandler<GetAllCurrencyExchangeRatesQuery, CurrencyExchangeRate?>
{
    public GetAllCurrencyExchangeRatesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CurrencyExchangeRate?>> Handle(GetAllCurrencyExchangeRatesQuery request, CancellationToken cancellationToken)
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

        if (request.TimeStampStart.HasValue)
        {
            query = query.Where(o => o.TimeStamp >= request.TimeStampStart.Value);
        }

        if (request.TimeStampEnd.HasValue)
        {
            query = query.Where(o => o.TimeStamp <= request.TimeStampEnd.Value);
        }

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        query = query
            .OrderBy(o => o.BaseCurrency.Name)
            .ThenBy(o => o.QuoteCurrency.Name)
            .ThenByDescending(o => o.TimeStamp);

        return await Task.FromResult(await query.ToArrayAsync());
    }
}

public class GetAllCurrencyExchangeRatesQuery : GetAllQuery<CurrencyExchangeRate?>
{
    public Guid? BaseCurrencyId { get; set; }

    public Guid? QuoteCurrencyId { get; set; }

    public DateTimeKind? DateTimeKind { get; set; }

    public DateTime? TimeStampStart { get; set; }

    public DateTime? TimeStampEnd { get; set; }
}
