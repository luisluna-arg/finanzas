using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetLatestCreditCardTransactionsFromStatementsQueryHandler : BaseCollectionHandler<GetLatestCreditCardTransactionsFromStatementsQuery, CreditCardTransaction>
{
    public GetLatestCreditCardTransactionsFromStatementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCardTransaction>>> ExecuteAsync(
        GetLatestCreditCardTransactionsFromStatementsQuery request, CancellationToken cancellationToken)
    {
        List<CreditCardTransaction> result;
        try
        {
            // Get the latest statement IDs for each credit card (or specific card if requested)
            var latestStatementIds = await DbContext.CreditCard
                .Where(cc => !cc.Deactivated)
                .Where(cc => !request.CreditCardId.HasValue || cc.Id == request.CreditCardId.Value)
                .Select(cc => cc.CurrentStatementId)
                .Where(statementId => statementId != null)
                .ToListAsync(cancellationToken);

            if (!latestStatementIds.Any())
            {
                result = new List<CreditCardTransaction>();
            }
            else
            {
                // Get transaction IDs that belong to those latest statements
                var latestTransactionIds = await DbContext.CreditCardStatementTransaction
                    .Where(st => latestStatementIds.Contains(st.CreditCardStatementId))
                    .Where(st => st.CreditCardTransactionId != null)
                    .Select(st => st.CreditCardTransactionId!.Value)
                    .ToListAsync(cancellationToken);

                // Now get the actual transactions with proper includes
                var transactionsQuery = DbContext.CreditCardTransaction
                    .Include(t => t.CreditCard)
                    .ThenInclude(cc => cc.Bank)
                    .Where(t => latestTransactionIds.Contains(t.Id))
                    .AsQueryable();

                if (!request.IncludeDeactivated)
                {
                    transactionsQuery = transactionsQuery.Where(t => !t.Deactivated);
                }

                // Apply pagination if requested
                if (request.PageSize.HasValue && request.PageSize > 0)
                {
                    var page = request.Page ?? 1;
                    var pageSize = request.PageSize.Value;

                    transactionsQuery = transactionsQuery
                        .OrderByDescending(t => t.Timestamp)
                        .ThenBy(t => t.CreditCard.Name)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize);
                }
                else
                {
                    transactionsQuery = transactionsQuery
                        .OrderByDescending(t => t.Timestamp)
                        .ThenBy(t => t.CreditCard.Name);
                }

                result = await transactionsQuery.ToListAsync(cancellationToken);
            }
        }
        catch
        {
            result = new();
        }

        return DataResult<List<CreditCardTransaction>>.Success(result);
    }
}

public class GetLatestCreditCardTransactionsFromStatementsQuery : GetAllQuery<CreditCardTransaction>
{
    public Guid? CreditCardId { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
