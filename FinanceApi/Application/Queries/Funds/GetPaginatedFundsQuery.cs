using FinanceApi.Application.Queries.Base;
using FinanceApi.Commons;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Funds;

public class GetPaginatedFundsQueryHandler : IRequestHandler<GetPaginatedFundsQuery, PaginatedResult<Fund?>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedFundsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PaginatedResult<Fund?>> Handle(GetPaginatedFundsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Fund?> query = dbContext.Set<Fund>()
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
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
            .ThenBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Fund?>(paginatedItems, page, pageSize, totalItems);
    }
}

public class GetPaginatedFundsQuery : GetPaginatedQuery<Fund?>
{
    public Guid? CurrencyId { get; set; }

    public Guid? BankId { get; set; }
}
