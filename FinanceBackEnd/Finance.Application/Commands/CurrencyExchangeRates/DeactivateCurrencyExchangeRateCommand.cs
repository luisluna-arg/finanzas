using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class DeactivateCurrencyExchangeRateCommandHandler : IRequestHandler<DeactivateCurrencyExchangeRateCommand, CurrencyExchangeRate?>
{
    private readonly IEntityService<CurrencyExchangeRate, Guid> service;

    public DeactivateCurrencyExchangeRateCommandHandler(
        IEntityService<CurrencyExchangeRate, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CurrencyExchangeRate?> Handle(DeactivateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateCurrencyExchangeRateCommand : IRequest<CurrencyExchangeRate?>
{
    public Guid Id { get; set; }
}
