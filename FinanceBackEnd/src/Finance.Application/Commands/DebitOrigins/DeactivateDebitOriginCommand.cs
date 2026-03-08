using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.DebitOrigins;

public class DeactivateDebitOriginCommand : BatchUpdateBaseCommand;

public class DeactivateDebitOriginCommandHandler(IEntityService<DebitOrigin, Guid> service) : ICommandHandler<DeactivateDebitOriginCommand>
{
    private readonly IEntityService<DebitOrigin, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(DeactivateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new DeactivateDebitOriginCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, true, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeactivateDebitOriginCommandValidator : BatchUpdateBaseCommandValidator<DeactivateDebitOriginCommand>;
