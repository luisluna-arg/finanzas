using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Commands.Incomes;

public class DeleteIncomesCommandHandler(IEntityService<Income, Guid> service)
    : ICommandHandler<DeleteIncomesCommand>
{
    public async Task<CommandResult> ExecuteAsync(DeleteIncomesCommand request, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public record DeleteIncomesCommand(Guid[] Ids) : ICommand<CommandResult>;
