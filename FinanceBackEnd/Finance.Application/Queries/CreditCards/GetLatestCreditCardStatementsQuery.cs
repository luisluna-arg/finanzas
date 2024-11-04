using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetLatestCreditCardStatementsQueryHandler : BaseCollectionHandler<GetLatestCreditCardStatementsQuery, CreditCardStatement>
{
    public GetLatestCreditCardStatementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CreditCardStatement>> Handle(
        GetLatestCreditCardStatementsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = DbContext.CreditCardStatement
                .Include(o => o.CreditCard)
                .ThenInclude(o => o.Bank).AsQueryable();

            DateTime[] dates;
            if (request.CreditCardId.HasValue)
            {
                query = query.Where(o => o.CreditCardId == request.CreditCardId.Value);
                dates = new DateTime[] { await query.MaxAsync(o => o.ClosureDate) };
            }
            else
            {
                dates = await query.GroupBy(o => o.CreditCardId).Select(o => o.Max(o => o.ClosureDate)).ToArrayAsync();
            }

            return await query.Where(o => dates.Contains(o.ClosureDate))
                .OrderByDescending(o => o.ClosureDate)
                .ThenBy(o => o.CreditCard.Name)
                .ToArrayAsync();
        }
        catch
        {
            return new CreditCardStatement[0];
        }
    }
}

public class GetLatestCreditCardStatementsQuery : GetAllQuery<CreditCardStatement>
{
    public Guid? CreditCardId { get; set; }
}
