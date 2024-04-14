using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Movements;

public class DeactivateMovementCommandHandler : IRequestHandler<DeactivateMovementCommand, Movement?>
{
    private readonly IEntityService<Movement, Guid> service;

    public DeactivateMovementCommandHandler(
        IEntityService<Movement, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Movement?> Handle(DeactivateMovementCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateMovementCommand : IRequest<Movement?>
{
    public Guid Id { get; set; }
}
