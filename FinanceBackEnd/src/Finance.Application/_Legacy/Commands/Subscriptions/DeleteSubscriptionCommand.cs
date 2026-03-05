using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Commands.Subscriptions.Base;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.Subscriptions;

namespace Finance.Application.Legacy.Commands.Subscriptions;

public class DeleteSubscriptionCommand : BatchSubscriptionUpdateBaseCommand;

public class DeleteSubscriptionCommandHandler(IEntityService<Subscription, Guid> service) : ICommandHandler<DeleteSubscriptionCommand, CommandResult>
{
    private readonly IEntityService<Subscription, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(DeleteSubscriptionCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new DeleteSubscriptionCommandValidator());
        await _service.DeleteAsync(command.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteSubscriptionCommandValidator : BatchSubscriptionUpdateBaseCommandValidator<DeleteSubscriptionCommand>;
