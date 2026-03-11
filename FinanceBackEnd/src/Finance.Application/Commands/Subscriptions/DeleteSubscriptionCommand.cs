using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Subscriptions;

namespace Finance.Application.Commands.Subscriptions;

public sealed class DeleteSubscriptionCommand : DeleteEntityCommand<Guid>;

public sealed class DeleteSubscriptionCommandHandler(IRepository<Subscription, Guid> repository)
    : DeleteEntityCommandHandler<Subscription, Guid, DeleteSubscriptionCommand, DeleteSubscriptionCommandValidator>(repository);

public sealed class DeleteSubscriptionCommandValidator()
    : DeleteEntityCommandValidator<DeleteSubscriptionCommand, Guid>();
