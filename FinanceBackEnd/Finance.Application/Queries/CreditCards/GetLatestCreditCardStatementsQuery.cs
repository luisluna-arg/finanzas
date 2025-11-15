using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetLatestCreditCardStatementsQueryHandler : BaseCollectionHandler<GetLatestCreditCardStatementsQuery, CreditCardStatement>
{
    public GetLatestCreditCardStatementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCardStatement>>> ExecuteAsync(
        GetLatestCreditCardStatementsQuery request, CancellationToken cancellationToken)
    {
        List<CreditCardStatement> result;
        try
        {
            var query = DbContext.CreditCardStatement
                .Include(o => o.CreditCard)
                .ThenInclude(o => o.Bank).AsQueryable();

            DateTime[] dates;
            if (request.CreditCardId.HasValue)
            {
                query = query.Where(o => o.CreditCardId == request.CreditCardId.Value);
                dates = [await query.MaxAsync(o => o.ClosureDate)];
            }
            else
            {
                dates = await query.GroupBy(o => o.CreditCardId).Select(o => o.Max(o => o.ClosureDate)).ToArrayAsync(cancellationToken);
            }

            result = await query.Where(o => dates.Contains(o.ClosureDate))
                    .OrderByDescending(o => o.ClosureDate)
                    .ThenBy(o => o.CreditCard.Name)
                    .ToListAsync(cancellationToken);
        }
        catch
        {
            result = new();
        }

        return DataResult<List<CreditCardStatement>>.Success(result);
    }
}

public class GetLatestCreditCardStatementsQuery : GetAllQuery<CreditCardStatement>
{
    public Guid? CreditCardId { get; set; }
}
