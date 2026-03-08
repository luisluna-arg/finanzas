using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Services;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Commands.Funds;

public class DeleteFundsCommandHandler(IEntityService<Fund, Guid> service) : ICommandHandler<DeleteFundsCommand, CommandResult>
{
    public async Task<CommandResult> ExecuteAsync(DeleteFundsCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNotValid(new DeleteFundsCommandValidator());
        await service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteFundsCommand : BatchUpdateBaseCommand;

public class DeleteFundsCommandValidator : BatchUpdateBaseCommandValidator<DeleteFundsCommand>;
