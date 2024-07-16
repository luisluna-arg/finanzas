using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class DeleteCreditCardMovementCommandHandler : IRequestHandler<DeleteCreditCardMovementCommand>
{
    private readonly IEntityService<CreditCardMovement, Guid> service;

    public DeleteCreditCardMovementCommandHandler(
        IEntityService<CreditCardMovement, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteCreditCardMovementCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteCreditCardMovementCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
