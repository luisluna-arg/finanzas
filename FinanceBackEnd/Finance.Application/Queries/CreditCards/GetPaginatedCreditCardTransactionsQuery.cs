using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commons;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetPaginatedCreditCardTransactionsQueryHandler
    : IQueryHandler<GetPaginatedCreditCardTransactionsQuery, PaginatedResult<CreditCardTransaction>>
{
    private readonly FinanceDbContext dbContext;

    public GetPaginatedCreditCardTransactionsQueryHandler(
        FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DataResult<PaginatedResult<CreditCardTransaction>>> ExecuteAsync(GetPaginatedCreditCardTransactionsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<CreditCardTransaction> query = dbContext
            .Set<CreditCardTransaction>()
            .Include(o => o.CreditCard)
            .ThenInclude(o => o.Bank)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.Where(o => o.Timestamp >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(o => o.Timestamp <= request.To.Value);
        }

        if (request.CreditCardId.HasValue)
        {
            query = query.Where(o => o.CreditCardId == request.CreditCardId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.BankId))
        {
            query = query.Where(o => o.CreditCard.BankId == Guid.Parse(request.BankId));
        }

        // Pagination
        int page = request.Page;
        int pageSize = request.PageSize;
        int totalItems = await query.CountAsync(cancellationToken);

        var paginatedItems = await query
            .OrderByDescending(o => o.Timestamp)
            .ThenBy(o => o.CreditCard.Name)
            .ThenBy(o => o.Concept)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedResult<CreditCardTransaction>(paginatedItems, page, pageSize, totalItems);

        return DataResult<PaginatedResult<CreditCardTransaction>>.Success(paginatedResult);
    }
}

public class GetPaginatedCreditCardTransactionsQuery : GetPaginatedQuery<CreditCardTransaction>
{
    public Guid? CreditCardId { get; set; }
    public string? BankId { get; set; }
}
