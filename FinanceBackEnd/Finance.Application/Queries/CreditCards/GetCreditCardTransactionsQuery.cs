using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardTransactionsQueryHandler : BaseCollectionHandler<GetCreditCardTransactionsQuery, CreditCardTransaction>
{
    public GetCreditCardTransactionsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCardTransaction>>> ExecuteAsync(GetCreditCardTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CreditCardTransaction
            .Include(o => o.CreditCard)
            .ThenInclude(o => o.Bank)
            .AsQueryable();

        if (request.CreditCardId.HasValue)
        {
            query = query.Where(o => o.CreditCardId == request.CreditCardId.Value);
        }

        if (request.From.HasValue)
        {
            query = query.Where(o => o.Timestamp >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(o => o.Timestamp <= request.To.Value);
        }

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<CreditCardTransaction>>.Success(
            await query
                .OrderByDescending(o => o.Timestamp)
                .ThenBy(o => o.CreditCard.Name)
                .ThenBy(o => o.Concept)
                .ToListAsync(cancellationToken)
        );
    }
}

public class GetCreditCardTransactionsQuery : GetAllQuery<CreditCardTransaction>
{
    public Guid? CreditCardId { get; set; }

    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }
}
