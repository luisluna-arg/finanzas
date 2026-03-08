using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.Subscriptions.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Subscriptions;

namespace Finance.Application.Commands.Subscriptions;

public class DeactivateSubscriptionCommand : BatchSubscriptionUpdateBaseCommand;

public class DeactivateSubscriptionCommandHandler(IEntityService<Subscription, Guid> service) : ICommandHandler<DeactivateSubscriptionCommand>
{
    private readonly IEntityService<Subscription, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(DeactivateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new DeactivateSubscriptionCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, true, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeactivateSubscriptionCommandValidator : BatchSubscriptionUpdateBaseCommandValidator<DeactivateSubscriptionCommand>;
