using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetLatestCreditCardMovementsQueryHandler : BaseCollectionHandler<GetLatestCreditCardMovementsQuery, CreditCardMovement>
{
    public GetLatestCreditCardMovementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CreditCardMovement>> Handle(GetLatestCreditCardMovementsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = DbContext.CreditCardMovement.Include(o => o.CreditCard).ThenInclude(o => o.Bank).AsQueryable();

            DateTime[] dates;
            if (request.CreditCardId.HasValue)
            {
                query = query.Where(o => o.CreditCardId == request.CreditCardId.Value);
                dates = new DateTime[] { await query.MaxAsync(o => o.TimeStamp) };
            }
            else
            {
                dates = await query.GroupBy(o => o.CreditCardId).Select(o => o.Max(o => o.TimeStamp)).ToArrayAsync();
            }

            return await query.Where(o => dates.Contains(o.TimeStamp))
                .OrderByDescending(o => o.TimeStamp)
                .ThenBy(o => o.CreditCard.Name)
                .ThenBy(o => o.Concept)
                .ToArrayAsync();
        }
        catch
        {
            return new CreditCardMovement[0];
        }
    }
}

public class GetLatestCreditCardMovementsQuery : GetAllQuery<CreditCardMovement>
{
    public Guid? CreditCardId { get; set; }
}
