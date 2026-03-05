using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Legacy.Commands.CurrencyExchangeRates;

public class ActivateCurrencyExchangeRateCommandHandler : ICommandHandler<ActivateCurrencyExchangeRateCommand, DataResult<CurrencyExchangeRate?>>
{
    private readonly IEntityService<CurrencyExchangeRate, Guid> _service;

    public ActivateCurrencyExchangeRateCommandHandler(
        IEntityService<CurrencyExchangeRate, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<CurrencyExchangeRate?>> ExecuteAsync(ActivateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        => DataResult<CurrencyExchangeRate?>.Success(await _service.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateCurrencyExchangeRateCommand : ICommand
{
    public Guid Id { get; set; }
}
