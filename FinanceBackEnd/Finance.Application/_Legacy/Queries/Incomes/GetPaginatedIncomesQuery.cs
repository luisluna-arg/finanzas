using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commons;
using Finance.Application.Legacy.Queries.Base;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Legacy.Queries.Incomes;

public class GetPaginatedIncomesQueryHandler : IQueryHandler<GetPaginatedIncomesQuery, PaginatedResult<Income>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedIncomesQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DataResult<PaginatedResult<Income>>> ExecuteAsync(GetPaginatedIncomesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Income> query = dbContext
            .Income
            .Include(o => o.Bank)
            .Include(o => o.Currency);

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
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
        int totalItems = await query.CountAsync(cancellationToken);
        int pageSize = request.PageSize > 0 ? request.PageSize : totalItems;
        int skip = (page - 1) * pageSize;
        skip = skip < 0 ? 0 : skip;

        var paginatedItems = await query
            .OrderByDescending(o => o.TimeStamp)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return DataResult<PaginatedResult<Income>>.Success(new PaginatedResult<Income>(paginatedItems, page, pageSize, totalItems));
    }
}

public class GetPaginatedIncomesQuery : GetPaginatedQuery<Income>
{
    public Guid? CurrencyId { get; set; }
    public Guid? BankId { get; set; }
}
