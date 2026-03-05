using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Legacy.Commands;

public class CreateSubscriptionPermissionsCommand : CreateResourcePermissionsCommand<Subscription, Guid, SubscriptionPermissions>;

public class CreateSubscriptionPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateSubscriptionPermissionsCommand, Subscription, Guid, SubscriptionPermissions>(dbContext)
{
    protected override async Task<Subscription?> QuerySource(CreateSubscriptionPermissionsCommand request, CancellationToken cancellationToken)
    {
        return await DbContext.Subscriptions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == request.ResourceId, cancellationToken);
    }
}
