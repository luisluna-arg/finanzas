using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Currencies;

public class ActivateCurrencyCommandHandler : IRequestHandler<ActivateCurrencyCommand, Currency?>
{
    private readonly IEntityService<Currency, Guid> service;

    public ActivateCurrencyCommandHandler(
        IEntityService<Currency, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Currency?> Handle(ActivateCurrencyCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateCurrencyCommand : IRequest<Currency?>
{
    public Guid Id { get; set; }
}
