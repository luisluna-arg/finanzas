using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class DeactivateCurrencyExchangeRateCommandHandler : ICommandHandler<DeactivateCurrencyExchangeRateCommand, DataResult<CurrencyExchangeRate?>>
{
    private readonly IEntityService<CurrencyExchangeRate, Guid> _service;

    public DeactivateCurrencyExchangeRateCommandHandler(
        IEntityService<CurrencyExchangeRate, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<CurrencyExchangeRate?>> ExecuteAsync(DeactivateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        => DataResult<CurrencyExchangeRate?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateCurrencyExchangeRateCommand : ICommand<DataResult<CurrencyExchangeRate?>>
{
    public Guid Id { get; set; }
}
