using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Movements;

public class DeleteMovementsCommandHandler : IRequestHandler<DeleteMovementsCommand>
{
    private readonly IEntityService<Movement, Guid> service;

    public DeleteMovementsCommandHandler(
        IEntityService<Movement, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteMovementsCommand request, CancellationToken cancellationToken)
        => await service.Delete(request.Ids);
}

public class DeleteMovementsCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
