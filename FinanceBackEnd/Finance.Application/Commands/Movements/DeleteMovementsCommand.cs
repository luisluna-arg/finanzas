using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Movements;

public class DeleteMovementsCommandHandler : IRequestHandler<DeleteMovementsCommand>
{
    private readonly IEntityService<Movement, Guid> service;

    public DeleteMovementsCommandHandler(
        IEntityService<Movement, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteMovementsCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteMovementsCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
