using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CreditCardMovements;

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

            return await query.Where(o => dates.Contains(o.TimeStamp)).OrderByDescending(o => o.TimeStamp).ThenBy(o => o.CreditCard.Name).ToArrayAsync();
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
