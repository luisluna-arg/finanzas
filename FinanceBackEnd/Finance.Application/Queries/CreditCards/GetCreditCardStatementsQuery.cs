using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardStatementQueryHandler : BaseCollectionHandler<GetCreditCardStatementsQuery, CreditCardStatement?>
{
    public GetCreditCardStatementQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CreditCardStatement?>> Handle(
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

        return await query
            .OrderByDescending(o => o.ClosureDate)
            .ThenBy(o => o.CreditCard.Name)
            .ToArrayAsync();
    }
}

public class GetCreditCardStatementsQuery : GetAllQuery<CreditCardStatement?>
{
    public Guid? CreditCardId { get; private set; }
}
