using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyExchangeRates;

public class DeactivateCurrencyExchangeRateCommandHandler : IRequestHandler<DeactivateCurrencyExchangeRateCommand, CurrencyExchangeRate?>
{
    private readonly IEntityService<CurrencyExchangeRate, Guid> service;

    public DeactivateCurrencyExchangeRateCommandHandler(
        IEntityService<CurrencyExchangeRate, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CurrencyExchangeRate?> Handle(DeactivateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, true);
}

public class DeactivateCurrencyExchangeRateCommand : IRequest<CurrencyExchangeRate?>
{
    public Guid Id { get; set; }
}
