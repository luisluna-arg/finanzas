using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.CurrencyConversions;

public class ActivateCurrencyConversionCommandHandler : ICommandHandler<ActivateCurrencyConversionCommand, DataResult<CurrencyConversion?>>
{
    private readonly IEntityService<CurrencyConversion, Guid> _service;

    public ActivateCurrencyConversionCommandHandler(
        IEntityService<CurrencyConversion, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<CurrencyConversion?>> ExecuteAsync(ActivateCurrencyConversionCommand request, CancellationToken cancellationToken)
        => DataResult<CurrencyConversion?>.Success(await _service.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateCurrencyConversionCommand : ICommand
{
    public Guid Id { get; set; }
}
