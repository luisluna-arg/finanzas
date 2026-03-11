using Finance.Application.Commands.Base;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Commands.Subscriptions;

public class DeactivateSubscriptionCommand : BatchUpdateBaseCommand;

public class DeactivateSubscriptionCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateSubscriptionCommand, DeactivateSubscriptionCommandValidator, Subscription, Guid>(dbContext);

public class DeactivateSubscriptionCommandValidator : BatchUpdateBaseCommandValidator<DeactivateSubscriptionCommand>;
