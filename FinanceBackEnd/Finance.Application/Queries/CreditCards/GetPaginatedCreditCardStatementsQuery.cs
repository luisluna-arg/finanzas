using Finance.Application.Queries.Base;
using Finance.Application.Commons;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetPaginatedCreditCardStatementsQueryHandler
    : IRequestHandler<GetPaginatedCreditCardStatementsQuery, PaginatedResult<CreditCardStatement>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedCreditCardStatementsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PaginatedResult<CreditCardStatement>> Handle(GetPaginatedCreditCardStatementsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<CreditCardStatement> query = dbContext
            .Set<CreditCardStatement>()
            .Include(c => c.CreditCard)
                .ThenInclude(c => c.Bank)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.Where(o => o.ClosureDate >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(o => o.ClosureDate <= request.To.Value);
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
            .OrderByDescending(o => o.ClosureDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedResult = new PaginatedResult<CreditCardStatement>(paginatedItems, page, pageSize, totalItems);

        return paginatedResult;
    }
}

public class GetPaginatedCreditCardStatementsQuery : GetPaginatedQuery<CreditCardStatement>
{
    public string? CreditCardId { get; set; }

    public string? BankId { get; set; }
}
