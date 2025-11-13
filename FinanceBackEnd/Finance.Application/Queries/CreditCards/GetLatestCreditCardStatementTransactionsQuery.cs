using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetLatestCreditCardStatementTransactionsQueryHandler : BaseCollectionHandler<GetLatestCreditCardStatementTransactionsQuery, CreditCardStatementTransaction>
{
    public GetLatestCreditCardStatementTransactionsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCardStatementTransaction>>> ExecuteAsync(
        GetLatestCreditCardStatementTransactionsQuery request, CancellationToken cancellationToken)
    {
        List<CreditCardStatementTransaction> result;
        try
        {
            // First, get the latest statement(s)
            var statementsQuery = DbContext.CreditCardStatement
                .Include(o => o.CreditCard)
                .ThenInclude(o => o.Bank)
                .AsQueryable();

            DateTime[] latestStatementDates;
            if (request.CreditCardId.HasValue)
            {
                statementsQuery = statementsQuery.Where(o => o.CreditCardId == request.CreditCardId.Value);
                var maxDate = await statementsQuery.MaxAsync(o => o.ClosureDate, cancellationToken);
                latestStatementDates = [maxDate];
            }
            else
            {
                latestStatementDates = await statementsQuery
                    .GroupBy(o => o.CreditCardId)
                    .Select(o => o.Max(s => s.ClosureDate))
                    .ToArrayAsync(cancellationToken);
            }

            var latestStatements = await statementsQuery
                .Where(o => latestStatementDates.Contains(o.ClosureDate))
                .ToListAsync(cancellationToken);

            var latestStatementIds = latestStatements.Select(s => s.Id).ToArray();

            // Now get the transactions from those latest statements
            var transactionsQuery = DbContext.CreditCardStatementTransaction
                .Include(o => o.CreditCardStatement)
                .ThenInclude(o => o.CreditCard)
                .ThenInclude(o => o.Bank)
                .Where(o => latestStatementIds.Contains(o.CreditCardStatementId))
                .AsQueryable();

            if (!request.IncludeDeactivated)
            {
                transactionsQuery = transactionsQuery.Where(o => !o.Deactivated);
            }

            // Apply pagination if requested
            if (request.PageSize.HasValue && request.PageSize > 0)
            {
                var page = request.Page ?? 1;
                var pageSize = request.PageSize.Value;

                transactionsQuery = transactionsQuery
                    .OrderByDescending(o => o.PostedDate)
                    .ThenBy(o => o.CreditCardStatement.CreditCard.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                transactionsQuery = transactionsQuery
                    .OrderByDescending(o => o.PostedDate)
                    .ThenBy(o => o.CreditCardStatement.CreditCard.Name);
            }

            result = await transactionsQuery.ToListAsync(cancellationToken);
        }
        catch
        {
            result = new();
        }

        return DataResult<List<CreditCardStatementTransaction>>.Success(result);
    }
}

public class GetLatestCreditCardStatementTransactionsQuery : GetAllQuery<CreditCardStatementTransaction>
{
    public Guid? CreditCardId { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}