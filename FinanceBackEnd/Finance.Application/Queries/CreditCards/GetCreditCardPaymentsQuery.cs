using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardPaymentsQueryHandler : BaseCollectionHandler<GetCreditCardPaymentsQuery, CreditCardPayment>
{
    public GetCreditCardPaymentsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCardPayment>>> ExecuteAsync(GetCreditCardPaymentsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CreditCardPayment
            .Include(o => o.Statement)
            .ThenInclude(o => o.CreditCard)
            .ThenInclude(o => o.Bank)
            .AsQueryable();

        if (request.StatementId.HasValue)
        {
            query = query.Where(o => o.StatementId == request.StatementId.Value);
        }

        if (request.CreditCardId.HasValue)
        {
            query = query.Where(o => o.Statement.CreditCardId == request.CreditCardId.Value);
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

        return DataResult<List<CreditCardPayment>>.Success(
            await query
                .OrderByDescending(o => o.Timestamp)
                .ThenBy(o => o.Statement.CreditCard.Name)
                .ToListAsync(cancellationToken)
        );
    }
}

public class GetCreditCardPaymentsQuery : GetAllQuery<CreditCardPayment>
{
    public Guid? StatementId { get; set; }
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
