using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commons;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Legacy.Queries.Subscriptions;

public class GetPaginatedSubscriptionsQuery : GetPaginatedQuery<Subscription>
{
    public Guid? CurrencyId { get; set; }
    public FrequencyEnum? Frequency { get; set; }
}

public class GetPaginatedSubscriptionsQueryHandler : IQueryHandler<GetPaginatedSubscriptionsQuery, PaginatedResult<Subscription>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedSubscriptionsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DataResult<PaginatedResult<Subscription>>> ExecuteAsync(GetPaginatedSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Subscription> query = dbContext
            .Subscriptions
            .Include(o => o.Currency)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(o => o.CurrencyId == request.CurrencyId.Value);
        }

        if (request.Frequency.HasValue)
        {
            query = query.Where(o => o.Frequency == request.Frequency.Value);
        }

        // Pagination
        int page = request.Page;
        int pageSize = request.PageSize;
        int totalItems = await query.CountAsync();

        var paginatedItems = await query
            .OrderBy(o => o.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return DataResult<PaginatedResult<Subscription>>.Success(new PaginatedResult<Subscription>(paginatedItems, page, pageSize, totalItems));
    }
}