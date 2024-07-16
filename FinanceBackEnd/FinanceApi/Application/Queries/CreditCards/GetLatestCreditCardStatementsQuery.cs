using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CreditCards;

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
