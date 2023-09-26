using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class ActivateCreditCardIssuerCommandHandler : IRequestHandler<ActivateCreditCardIssuerCommand, CreditCardIssuer?>
{
    private readonly IEntityService<CreditCardIssuer, Guid> service;

    public ActivateCreditCardIssuerCommandHandler(
        IEntityService<CreditCardIssuer, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CreditCardIssuer?> Handle(ActivateCreditCardIssuerCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, false);
}

public class ActivateCreditCardIssuerCommand : IRequest<CreditCardIssuer?>
{
    public Guid Id { get; set; }
}
