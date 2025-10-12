using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardStatementQueryHandler : BaseCollectionHandler<GetCreditCardStatementsQuery, CreditCardStatement>
{
    public GetCreditCardStatementQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCardStatement>>> ExecuteAsync(
        GetCreditCardStatementsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext
            .CreditCardStatement
            .Include(o => o.CreditCard)
                .ThenInclude(o => o.Bank)
            .AsQueryable();

        if (request.CreditCardId.HasValue)
        {
            query = query.Where(o => o.CreditCardId == request.CreditCardId);
        }

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<CreditCardStatement>>.Success(await query
            .OrderByDescending(o => o.ClosureDate)
            .ThenBy(o => o.CreditCard.Name)
            .ToListAsync(cancellationToken));
    }
}

public class GetCreditCardStatementsQuery : GetAllQuery<CreditCardStatement>
{
    public Guid? CreditCardId { get; private set; }
}
