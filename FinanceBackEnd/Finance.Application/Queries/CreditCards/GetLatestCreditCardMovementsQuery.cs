using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetLatestCreditCardMovementsQueryHandler : BaseCollectionHandler<GetLatestCreditCardMovementsQuery, CreditCardMovement>
{
    public GetLatestCreditCardMovementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCardMovement>>> ExecuteAsync(GetLatestCreditCardMovementsQuery request, CancellationToken cancellationToken)
    {
        List<CreditCardMovement> result;
        try
        {
            var query = DbContext.CreditCardMovement.Include(o => o.CreditCard).ThenInclude(o => o.Bank).AsQueryable();

            DateTime[] dates;
            if (request.CreditCardId.HasValue)
            {
                query = query.Where(o => o.CreditCardId == request.CreditCardId.Value);
                dates = [await query.MaxAsync(o => o.TimeStamp)];
            }
            else
            {
                dates = await query.GroupBy(o => o.CreditCardId).Select(o => o.Max(o => o.TimeStamp)).ToArrayAsync(cancellationToken);
            }

            result = await query.Where(o => dates.Contains(o.TimeStamp))
                .OrderByDescending(o => o.TimeStamp)
                .ThenBy(o => o.CreditCard.Name)
                .ThenBy(o => o.Concept)
                .ToListAsync(cancellationToken);
        }
        catch
        {
            result = [];
        }

        return DataResult<List<CreditCardMovement>>.Success(result);
    }
}

public class GetLatestCreditCardMovementsQuery : GetAllQuery<CreditCardMovement>
{
    public Guid? CreditCardId { get; set; }
}
