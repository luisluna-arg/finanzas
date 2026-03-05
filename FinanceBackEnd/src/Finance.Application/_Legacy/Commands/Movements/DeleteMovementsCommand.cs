using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.Movements;

namespace Finance.Application.Legacy.Commands.Movements;

public class DeleteMovementsCommandHandler : ICommandHandler<DeleteMovementsCommand>
{
    private readonly IEntityService<Movement, Guid> _service;

    public DeleteMovementsCommandHandler(IEntityService<Movement, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteMovementsCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteMovementsCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
