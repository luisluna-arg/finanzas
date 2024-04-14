using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CreditCards;

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
