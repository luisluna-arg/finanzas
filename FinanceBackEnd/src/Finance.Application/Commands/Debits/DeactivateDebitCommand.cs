using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.Debits;

public class DeactivateDebitCommand : BatchUpdateBaseCommand;

public class DeactivateDebitCommandHandler(IEntityService<Debit, Guid> service) : ICommandHandler<DeactivateDebitCommand>
{
    private readonly IEntityService<Debit, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(DeactivateDebitCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new DeactivateDebitCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, true, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeactivateDebitCommandValidator : BatchUpdateBaseCommandValidator<DeactivateDebitCommand>;
