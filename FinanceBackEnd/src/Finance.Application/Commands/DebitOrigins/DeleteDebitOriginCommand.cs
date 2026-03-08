using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.DebitOrigins;

public class DeleteDebitOriginCommand : BatchUpdateBaseCommand;

public class DeleteDebitOriginCommandHandler(IEntityService<DebitOrigin, Guid> service) : ICommandHandler<DeleteDebitOriginCommand, CommandResult>
{
    private readonly IEntityService<DebitOrigin, Guid> _service = service;

    public async Task<CommandResult> ExecuteAsync(DeleteDebitOriginCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new DeleteDebitOriginCommandValidator());
        await _service.DeleteAsync(command.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteDebitOriginCommandValidator : BatchUpdateBaseCommandValidator<DeleteDebitOriginCommand>;
