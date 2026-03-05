using Finance.Application.Legacy.Queries.Resources;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Legacy.Queries.Subscriptions;

public class GetSubscriptionOwnershipQuery(Guid id)
    : BaseGetResourcePermissionsWithIdQuery<Subscription, Guid, SubscriptionPermissions>(id);

public class GetSubscriptionOwnershipQueryHandler(FinanceDbContext dbContext)
    : BaseGetResourcePermissionsWithIdQueryHandler<Subscription, Guid, SubscriptionPermissions>(dbContext);
