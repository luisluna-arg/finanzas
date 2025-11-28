using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Subscriptions.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Subscriptions;

namespace Finance.Application.Commands.Subscriptions;

public class ActivateSubscriptionCommand : BatchSubscriptionUpdateBaseCommand;

public class ActivateSubscriptionCommandHandler(IEntityService<Subscription, Guid> service) : ICommandHandler<ActivateSubscriptionCommand>
{
    private readonly IEntityService<Subscription, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(ActivateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new ActivateSubscriptionCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, false, cancellationToken);
        return CommandResult.Success();
    }
}

public class ActivateSubscriptionCommandValidator : BatchSubscriptionUpdateBaseCommandValidator<ActivateSubscriptionCommand>;
