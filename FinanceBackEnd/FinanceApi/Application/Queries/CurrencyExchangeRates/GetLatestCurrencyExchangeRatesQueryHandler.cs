using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CurrencyExchangeRates;

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

        var groupResult = query.GroupBy(
            child => new { child.BaseCurrencyId, child.QuoteCurrencyId },
            (key, group) =>
                group
                    .OrderBy(o => o.BaseCurrency.Name)
                    .ThenBy(o => o.QuoteCurrency.Name)
                    .ThenByDescending(o => o.TimeStamp)
                    .AsEnumerable());

        return await Task.FromResult(await groupResult.Select(o => o.First()).ToArrayAsync());
    }
}

public class GetLatestCurrencyExchangeRatesQuery : GetAllQuery<CurrencyExchangeRate?>
{
    public Guid? BaseCurrencyId { get; set; }

    public Guid? QuoteCurrencyId { get; set; }
}
