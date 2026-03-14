using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commons;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Movements;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Movements;

public class GetPaginatedMovementsQueryHandler : IQueryHandler<GetPaginatedMovementsQuery, PaginatedResult<Movement?>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedMovementsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DataResult<PaginatedResult<Movement?>>> ExecuteAsync(GetPaginatedMovementsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Movement> query = dbContext.Set<Movement>()
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

        if (!string.IsNullOrWhiteSpace(request.AppModuleId))
        {
            query = query.Where(o => o.AppModuleId == Guid.Parse(request.AppModuleId));
        }

        if (!string.IsNullOrWhiteSpace(request.BankId))
        {
            query = query.Where(o => o.BankId == Guid.Parse(request.BankId));
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

        var paginatedResult = new PaginatedResult<Movement?>(paginatedItems, page, pageSize, totalItems);

        return DataResult<PaginatedResult<Movement?>>.Success(paginatedResult);
    }
}

public class GetPaginatedMovementsQuery : GetPaginatedQuery<Movement?>
{
    public string? AppModuleId { get; set; }
    public string? BankId { get; set; }
}
