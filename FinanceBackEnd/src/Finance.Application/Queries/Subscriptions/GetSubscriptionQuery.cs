using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Subscriptions;

public class GetSubscriptionQuery : GetSingleByIdQuery<Subscription?, Guid>;

public class GetSubscriptionQueryHandler(FinanceDbContext db) : BaseQueryHandler<GetSubscriptionQuery, Subscription?>(db)
{
    public override async Task<DataResult<Subscription?>> ExecuteAsync(GetSubscriptionQuery request, CancellationToken cancellationToken)
        => DataResult<Subscription?>.Success(await DbContext.Subscriptions.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken));
}
