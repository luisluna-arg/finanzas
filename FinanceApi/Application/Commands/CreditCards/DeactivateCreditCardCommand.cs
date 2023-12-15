using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class DeactivateCreditCardCommandHandler : IRequestHandler<DeactivateCreditCardCommand, CreditCard?>
{
    private readonly IEntityService<CreditCard, Guid> service;

    public DeactivateCreditCardCommandHandler(
        IEntityService<CreditCard, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CreditCard?> Handle(DeactivateCreditCardCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, true);
}

public class DeactivateCreditCardCommand : IRequest<CreditCard?>
{
    public Guid Id { get; set; }
}
