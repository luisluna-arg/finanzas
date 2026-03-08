using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.Debits;

public class ActivateDebitCommand : BatchUpdateBaseCommand;

public class ActivateDebitCommandHandler(IEntityService<Debit, Guid> service) : ICommandHandler<ActivateDebitCommand>
{
    private readonly IEntityService<Debit, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(ActivateDebitCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new ActivateDebitCommandValidator());
        await _service.SetDeactivatedAsync(command.Ids, false, cancellationToken);
        return CommandResult.Success();
    }
}

public class ActivateDebitCommandValidator : BatchUpdateBaseCommandValidator<ActivateDebitCommand>;
