using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.DebitOrigins;

public class ActivateDebitOriginCommand : BatchUpdateBaseCommand;

public class ActivateDebitOriginCommandHandler(IEntityService<DebitOrigin, Guid> service) : ICommandHandler<ActivateDebitOriginCommand>
{
    private readonly IEntityService<DebitOrigin, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(ActivateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new ActivateDebitOriginCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, false, cancellationToken);
        return CommandResult.Success();
    }
}

public class ActivateDebitOriginCommandValidator : BatchUpdateBaseCommandValidator<ActivateDebitOriginCommand>;
