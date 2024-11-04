using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class ActivateCurrencyExchangeRateCommandHandler : IRequestHandler<ActivateCurrencyExchangeRateCommand, CurrencyExchangeRate?>
{
    private readonly IEntityService<CurrencyExchangeRate, Guid> service;

    public ActivateCurrencyExchangeRateCommandHandler(
        IEntityService<CurrencyExchangeRate, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CurrencyExchangeRate?> Handle(ActivateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateCurrencyExchangeRateCommand : IRequest<CurrencyExchangeRate?>
{
    public Guid Id { get; set; }
}
