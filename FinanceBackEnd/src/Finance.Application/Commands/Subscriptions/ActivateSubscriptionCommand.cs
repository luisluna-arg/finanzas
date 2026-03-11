using Finance.Application.Commands.Base;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Commands.Subscriptions;

public class ActivateSubscriptionCommand : BatchUpdateBaseCommand;

public class ActivateSubscriptionCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateSubscriptionCommand, ActivateSubscriptionCommandValidator, Subscription, Guid>(dbContext);

public class ActivateSubscriptionCommandValidator : BatchUpdateBaseCommandValidator<ActivateSubscriptionCommand>;
