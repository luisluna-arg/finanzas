using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.CurrencyConversions;

public class DeactivateCurrencyConversionCommandHandler : ICommandHandler<DeactivateCurrencyConversionCommand, DataResult<CurrencyConversion?>>
{
    private readonly IEntityService<CurrencyConversion, Guid> _service;

    public DeactivateCurrencyConversionCommandHandler(
        IEntityService<CurrencyConversion, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<CurrencyConversion?>> ExecuteAsync(DeactivateCurrencyConversionCommand request, CancellationToken cancellationToken)
        => DataResult<CurrencyConversion?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateCurrencyConversionCommand : ICommand<DataResult<CurrencyConversion?>>
{
    public Guid Id { get; set; }
}
