using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class ActivateCreditCardCommandHandler : IRequestHandler<ActivateCreditCardCommand, CreditCard?>
{
    private readonly IEntityService<CreditCard, Guid> service;

    public ActivateCreditCardCommandHandler(
        IEntityService<CreditCard, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CreditCard?> Handle(ActivateCreditCardCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, false);
}

public class ActivateCreditCardCommand : IRequest<CreditCard?>
{
    public Guid Id { get; set; }
}
