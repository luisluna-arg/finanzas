using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetLatestCreditCardTransactionsFromStatementsQueryHandler : BaseCollectionQueryHandler<GetLatestCreditCardTransactionsFromStatementsQuery, CreditCardTransaction>
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
            var statementsQuery = DbContext.CreditCardStatement
                .Where(s => !s.Deactivated)
                .Where(s => !request.CreditCardId.HasValue || s.CreditCardId == request.CreditCardId.Value)
                .AsQueryable();

            var statements = await statementsQuery
                .Select(s => new { s.Id, s.CreditCardId, s.ClosureDate, s.ExpiringDate })
                .ToListAsync(cancellationToken);

            var today = DateTime.UtcNow.Date;
            var latestStatementIds = statements
                .GroupBy(s => s.CreditCardId)
                .Select(g => g.OrderByDescending(s => s.ClosureDate).First())
                .Where(s => request.IncludeExpiredStatements || s.ExpiringDate.Date >= today)
                .Select(s => s.Id)
                .ToList();

            if (latestStatementIds.Count == 0)
            {
                result = new();
            }
            else
            {
                var latestTransactionIds = await DbContext.CreditCardStatementTransaction
                    .Where(st => latestStatementIds.Contains(st.StatementId))
                    .Where(st => st.CreditCardTransactionId != null)
                    .Select(st => st.CreditCardTransactionId!.Value)
                    .ToListAsync(cancellationToken);

                var transactionsQuery = DbContext.CreditCardTransaction
                    .Include(t => t.CreditCard)
                    .ThenInclude(cc => cc.Bank)
                    .Include(t => t.Currency)
                    .ThenInclude(c => c.Symbols)
                    .Where(t => latestTransactionIds.Contains(t.Id))
                    .AsQueryable();

                if (!request.IncludeDeactivated)
                {
                    transactionsQuery = transactionsQuery.Where(t => !t.Deactivated);
                }

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
    public bool IncludeExpiredStatements { get; set; }
}
