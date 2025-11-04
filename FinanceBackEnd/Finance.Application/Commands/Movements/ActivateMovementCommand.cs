using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models;

namespace Finance.Application.Commands.Movements;

public class ActivateMovementCommandHandler : ICommandHandler<ActivateMovementCommand, DataResult<Movement?>>
{
    private readonly IEntityService<Movement, Guid> _service;

    public ActivateMovementCommandHandler(
        IEntityService<Movement, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<Movement?>> ExecuteAsync(ActivateMovementCommand request, CancellationToken cancellationToken)
        => DataResult<Movement?>.Success(await _service.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateMovementCommand : ICommand
{
    public Guid Id { get; set; }
}
