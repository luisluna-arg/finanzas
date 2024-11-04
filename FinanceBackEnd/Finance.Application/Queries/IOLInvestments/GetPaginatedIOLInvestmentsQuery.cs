using Finance.Application.Queries.Base;
using Finance.Application.Commons;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.IOLInvestments;

public class GetPaginatedIOLInvestmentsQueryHandler : IRequestHandler<GetPaginatedIOLInvestmentsQuery, PaginatedResult<IOLInvestment?>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedIOLInvestmentsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PaginatedResult<IOLInvestment?>> Handle(GetPaginatedIOLInvestmentsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<IOLInvestment> query = dbContext.Set<IOLInvestment>()
            .Include(o => o.Asset)
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

        if (!string.IsNullOrWhiteSpace(request.AssetId))
        {
            query = query.Where(o => o.AssetId == Guid.Parse(request.AssetId));
        }

        // Pagination
        int page = request.Page;
        int pageSize = request.PageSize;
        int totalItems = await query.CountAsync();

        var paginatedItems = await query
            .OrderByDescending(o => o.TimeStamp)
            .ThenBy(o => o.Asset.Symbol)
            .ThenBy(o => o.Asset.Description)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<IOLInvestment?>(paginatedItems, page, pageSize, totalItems);
    }
}

public class GetPaginatedIOLInvestmentsQuery : GetPaginatedQuery<IOLInvestment?>
{
    public string? AssetId { get; set; }
}
