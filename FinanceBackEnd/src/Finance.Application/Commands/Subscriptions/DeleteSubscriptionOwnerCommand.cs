
using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Commands.Subscriptions;

public sealed class DeleteSubscriptionOwnerCommand
    : DeleteEntityOwnerCommand<Subscription, Guid, SubscriptionPermissions>;

public sealed class DeleteSubscriptionOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteSubscriptionOwnerCommand, Subscription, Guid, SubscriptionPermissions>(dbContext);
