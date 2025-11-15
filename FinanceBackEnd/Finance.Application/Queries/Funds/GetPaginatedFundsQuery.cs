using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commons;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Funds;

public class GetPaginatedFundsQueryHandler : IQueryHandler<GetPaginatedFundsQuery, PaginatedResult<Fund>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedFundsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DataResult<PaginatedResult<Fund>>> ExecuteAsync(GetPaginatedFundsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Fund> query = dbContext
            .Fund
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", Application.Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Application.Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(o => o.CurrencyId == request.CurrencyId.Value);
        }

        if (request.BankId.HasValue)
        {
            query = query.Where(o => o.BankId == request.BankId.Value);
        }

        // Pagination
        int page = request.Page;
        int pageSize = request.PageSize;
        int totalItems = await query.CountAsync();

        var paginatedItems = await query
            .OrderByDescending(o => o.TimeStamp)
            .ThenByDescending(o => o.CreatedAt)
            .ThenBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return DataResult<PaginatedResult<Fund>>.Success(new PaginatedResult<Fund>(paginatedItems, page, pageSize, totalItems));
    }
}

public class GetPaginatedFundsQuery : GetPaginatedQuery<Fund>
{
    public Guid? CurrencyId { get; set; }
    public Guid? BankId { get; set; }
}
