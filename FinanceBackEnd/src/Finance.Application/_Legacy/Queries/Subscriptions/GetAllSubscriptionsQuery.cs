using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Legacy.Queries.Subscriptions;

public class GetAllSubscriptionsQuery : GetAllQuery<Subscription>;

public class GetAllSubscriptionsQueryHandler(FinanceDbContext dbContext) : BaseCollectionQueryHandler<GetAllSubscriptionsQuery, Subscription>(dbContext)
{
    public override async Task<DataResult<List<Subscription>>> ExecuteAsync(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Subscriptions
            .OrderBy(o => o.Name)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<Subscription>>.Success(await query.ToListAsync(cancellationToken));
    }
}
