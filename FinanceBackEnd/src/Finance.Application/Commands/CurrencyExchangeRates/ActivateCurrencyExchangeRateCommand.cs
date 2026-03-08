using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class ActivateCurrencyExchangeRateCommandHandler(IEntityService<CurrencyExchangeRate, Guid> service)
    : ICommandHandler<ActivateCurrencyExchangeRateCommand, DataResult<CurrencyExchangeRate?>>
{
    public async Task<DataResult<CurrencyExchangeRate?>> ExecuteAsync(
        ActivateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        => DataResult<CurrencyExchangeRate?>.Success(await service.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateCurrencyExchangeRateCommand : ICommand<DataResult<CurrencyExchangeRate?>>
{
    public Guid Id { get; set; }
}
