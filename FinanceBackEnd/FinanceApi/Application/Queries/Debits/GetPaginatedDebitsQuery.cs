using FinanceApi.Application.Queries.Base;
using FinanceApi.Commons;
using FinanceApi.Domain;
using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Debits;

public class GetPaginatedDebitsQueryHandler : IRequestHandler<GetPaginatedDebitsQuery, PaginatedResult<Debit>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedDebitsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PaginatedResult<Debit>> Handle(GetPaginatedDebitsQuery request, CancellationToken cancellationToken)
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
            query = query.Where(o => o.Origin.AppModule.Type.Id == (int)request.AppModuleType.Value);
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
            .ToListAsync();

        var paginatedResult = new PaginatedResult<Debit>(paginatedItems, page, pageSize, totalItems);

        return paginatedResult;
    }
}

public class GetPaginatedDebitsQuery : GetPaginatedQuery<Debit>
{
    public Guid? AppModuleId { get; set; }

    public Guid? OriginId { get; set; }

    public AppModuleTypeEnum? AppModuleType { get; set; }

    public FrequencyEnum Frequency { get; set; }
}
