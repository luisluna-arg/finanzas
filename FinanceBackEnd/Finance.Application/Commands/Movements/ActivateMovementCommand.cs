using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Movements;

public class ActivateMovementCommandHandler : IRequestHandler<ActivateMovementCommand, Movement?>
{
    private readonly IEntityService<Movement, Guid> service;

    public ActivateMovementCommandHandler(
        IEntityService<Movement, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Movement?> Handle(ActivateMovementCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateMovementCommand : IRequest<Movement?>
{
    public Guid Id { get; set; }
}
