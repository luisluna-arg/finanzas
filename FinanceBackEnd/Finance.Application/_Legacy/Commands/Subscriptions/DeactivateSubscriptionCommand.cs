using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Commands.Subscriptions.Base;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.Subscriptions;

namespace Finance.Application.Legacy.Commands.Subscriptions;

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
