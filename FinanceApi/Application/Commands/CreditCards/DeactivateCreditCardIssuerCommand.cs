using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class DeactivateCreditCardIssuerCommandHandler : IRequestHandler<DeactivateCreditCardIssuerCommand, CreditCardIssuer?>
{
    private readonly IEntityService<CreditCardIssuer, Guid> service;

    public DeactivateCreditCardIssuerCommandHandler(
        IEntityService<CreditCardIssuer, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CreditCardIssuer?> Handle(DeactivateCreditCardIssuerCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, true);
}

public class DeactivateCreditCardIssuerCommand : IRequest<CreditCardIssuer?>
{
    public Guid Id { get; set; }
}
