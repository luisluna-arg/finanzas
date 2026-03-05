using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Legacy.Commands.Subscriptions;

public class CreateSubscriptionOwnershipCommand : CreateResourcePermissionsCommand<Subscription, Guid, SubscriptionPermissions>;

public class CreateSubscriptionOwnershipCommandHandler(FinanceDbContext dbContext)

    : CreateResourcePermissionsCommandHandler<CreateSubscriptionOwnershipCommand, Subscription, Guid, SubscriptionPermissions>(dbContext)
{
    protected override async Task<Subscription?> QuerySource(CreateSubscriptionOwnershipCommand request, CancellationToken cancellationToken)
        => await DbContext.Subscriptions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.ResourceId, cancellationToken);
}
