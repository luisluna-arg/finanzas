using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.Debits;

public class DeleteDebitCommand : BatchUpdateBaseCommand;

public class DeleteDebitCommandHandler(IEntityService<Debit, Guid> service) : ICommandHandler<DeleteDebitCommand, CommandResult>
{
    private readonly IEntityService<Debit, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(DeleteDebitCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new DeleteDebitCommandValidator());
        await _service.DeleteAsync(command.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteDebitCommandValidator : BatchUpdateBaseCommandValidator<DeleteDebitCommand>;
