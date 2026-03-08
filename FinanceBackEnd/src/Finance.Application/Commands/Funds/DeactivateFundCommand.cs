using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Commands.Funds;

public class DeactivateFundCommand : BatchUpdateBaseCommand;

public class DeactivateFundCommandHandler(IEntityService<Fund, Guid> service) : ICommandHandler<DeactivateFundCommand>
{
    private readonly IEntityService<Fund, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(DeactivateFundCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new DeactivateFundCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, true, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeactivateFundCommandValidator : BatchUpdateBaseCommandValidator<DeactivateFundCommand>;
