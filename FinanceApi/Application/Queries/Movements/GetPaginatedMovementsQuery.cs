using FinanceApi.Commons;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Movements;

public class GetPaginatedMovementsQueryHandler : IRequestHandler<GetPaginatedMovementsQuery, PaginatedResult<Movement?>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedMovementsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PaginatedResult<Movement?>> Handle(GetPaginatedMovementsQuery request, CancellationToken cancellationToken)
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
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedResult = new PaginatedResult<Movement?>(paginatedItems, page, pageSize, totalItems);

        return paginatedResult;
    }
}

public class GetPaginatedMovementsQuery : IRequest<PaginatedResult<Movement?>>
{
    public bool IncludeDeactivated { get; set; }

    public string? AppModuleId { get; set; }

    public string? BankId { get; set; }

    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }

    public int Page { get; set; } // Current page number

    public int PageSize { get; set; } // Number of items per page
}
