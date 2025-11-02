using Finance.Application.Queries.Base;
using Finance.Application.Commons;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetPaginatedCurrencyExchangeRatesQueryHandler : IQueryHandler<GetPaginatedCurrencyExchangeRatesQuery, PaginatedResult<CurrencyExchangeRate?>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedCurrencyExchangeRatesQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DataResult<PaginatedResult<CurrencyExchangeRate?>>> ExecuteAsync(GetPaginatedCurrencyExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<CurrencyExchangeRate> query = dbContext.Set<CurrencyExchangeRate>()
            .Include(o => o.BaseCurrency)
            .Include(o => o.QuoteCurrency)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.Where(o => o.TimeStamp >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(o => o.TimeStamp <= request.To.Value);
        }

        if (request.BaseCurrencyId.HasValue)
        {
            query = query.Where(o => o.BaseCurrencyId == request.BaseCurrencyId.Value);
        }

        if (request.QuoteCurrencyId.HasValue)
        {
            query = query.Where(o => o.QuoteCurrencyId == request.QuoteCurrencyId);
        }

        // Pagination
        int page = request.Page;
        int pageSize = request.PageSize;
        int totalItems = await query.CountAsync();

        var paginatedItems = await query
            .OrderByDescending(o => o.TimeStamp)
            .ThenBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedResult<CurrencyExchangeRate?>(paginatedItems, page, pageSize, totalItems);

        return DataResult<PaginatedResult<CurrencyExchangeRate?>>.Success(paginatedResult);
    }
}

public class GetPaginatedCurrencyExchangeRatesQuery : GetPaginatedQuery<CurrencyExchangeRate?>
{
    public Guid? BaseCurrencyId { get; set; }
    public Guid? QuoteCurrencyId { get; set; }
    public DateTimeKind? DateTimeKind { get; set; }
    public DateTime? TimeStampStart { get; set; }
    public DateTime? TimeStampEnd { get; set; }
}
