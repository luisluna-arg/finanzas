using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.Movements;

namespace Finance.Application.Legacy.Commands.Movements;

public class DeactivateMovementCommandHandler : ICommandHandler<DeactivateMovementCommand, DataResult<Movement?>>
{
    private readonly IEntityService<Movement, Guid> _service;

    public DeactivateMovementCommandHandler(
        IEntityService<Movement, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<Movement?>> ExecuteAsync(DeactivateMovementCommand request, CancellationToken cancellationToken)
        => DataResult<Movement?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateMovementCommand : ICommand<DataResult<Movement?>>
{
    public Guid Id { get; set; }
}
