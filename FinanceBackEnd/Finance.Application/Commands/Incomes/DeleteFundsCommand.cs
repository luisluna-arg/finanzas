using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Commands.Incomes;

public class DeleteIncomesCommandHandler : ICommandHandler<DeleteIncomesCommand>
{
    private readonly IEntityService<Income, Guid> _service;

    public DeleteIncomesCommandHandler(
        IEntityService<Income, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteIncomesCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteIncomesCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
