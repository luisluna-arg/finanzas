using Finance.Application.Queries.Base;
using Finance.Application.Commons;
using Finance.Domain.Enums;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Debits;

public class GetPaginatedDebitsQueryHandler : IQueryHandler<GetPaginatedDebitsQuery, PaginatedResult<Debit>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedDebitsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DataResult<PaginatedResult<Debit>>> ExecuteAsync(GetPaginatedDebitsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Debit> query = dbContext.Set<Debit>()
            .Include(o => o.Origin)
                .ThenInclude(o => o.AppModule)
                    .ThenInclude(o => o.Type)
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

        if (request.AppModuleId.HasValue)
        {
            query = query.Where(o => o.Origin.AppModuleId == request.AppModuleId.Value);
        }

        if (request.OriginId.HasValue)
        {
            query = query.Where(o => o.OriginId == request.OriginId.Value);
        }

        if (request.AppModuleType.HasValue)
        {
            query = query.Where(o => o.Origin.AppModule.Type.Id == request.AppModuleType.Value);
        }

        // Pagination
        int page = request.Page;
        int pageSize = request.PageSize;
        int totalItems = await query.CountAsync();

        var paginatedItems = await query
            .OrderByDescending(o => o.TimeStamp)
            .ThenBy(o => o.Origin.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedResult<Debit>(paginatedItems, page, pageSize, totalItems);

        return DataResult<PaginatedResult<Debit>>.Success(paginatedResult);
    }
}

public class GetPaginatedDebitsQuery : GetPaginatedQuery<Debit>
{
    public Guid? AppModuleId { get; set; }

    public Guid? OriginId { get; set; }

    public AppModuleTypeEnum? AppModuleType { get; set; }

    public FrequencyEnum Frequency { get; set; }
}
