using FinanceApi.Application.Queries.Base;
using FinanceApi.Commons;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CreditCards;

public class GetPaginatedCreditCardMovementsQueryHandler : IRequestHandler<GetPaginatedCreditCardMovementsQuery, PaginatedResult<CreditCardMovement?>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedCreditCardMovementsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PaginatedResult<CreditCardMovement?>> Handle(GetPaginatedCreditCardMovementsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<CreditCardMovement> query = dbContext.Set<CreditCardMovement>()
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

        if (!string.IsNullOrWhiteSpace(request.CreditCardId))
        {
            query = query.Where(o => o.CreditCardId == Guid.Parse(request.CreditCardId));
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

        var paginatedResult = new PaginatedResult<CreditCardMovement?>(paginatedItems, page, pageSize, totalItems);

        return paginatedResult;
    }
}

public class GetPaginatedCreditCardMovementsQuery : GetPaginatedQuery<CreditCardMovement?>
{
    public string? CreditCardId { get; set; }

    public string? BankId { get; set; }
}
