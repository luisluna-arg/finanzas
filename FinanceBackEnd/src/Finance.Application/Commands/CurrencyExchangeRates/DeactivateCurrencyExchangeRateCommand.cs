using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class DeactivateCurrencyExchangeRateCommand : ICommand<DataResult<CurrencyExchangeRate?>>
{
    public Guid Id { get; set; }
}

public class DeactivateCurrencyExchangeRateCommandHandler(IEntityService<CurrencyExchangeRate, Guid> service)
    : ICommandHandler<DeactivateCurrencyExchangeRateCommand, DataResult<CurrencyExchangeRate?>>
{
    public async Task<DataResult<CurrencyExchangeRate?>> ExecuteAsync(
        DeactivateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        => DataResult<CurrencyExchangeRate?>.Success(await service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}
