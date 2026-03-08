using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Commands.Funds;

public class ActivateFundCommand : BatchUpdateBaseCommand;

public class ActivateFundCommandHandler(IEntityService<Fund, Guid> service) : ICommandHandler<ActivateFundCommand>
{
    private readonly IEntityService<Fund, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(ActivateFundCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new ActivateFundCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, false, cancellationToken);
        return CommandResult.Success();
    }
}

public class ActivateFundCommandValidator : BatchUpdateBaseCommandValidator<ActivateFundCommand>;
