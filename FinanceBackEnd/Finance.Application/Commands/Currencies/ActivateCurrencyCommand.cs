using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models;

namespace Finance.Application.Commands.Currencies;

public class ActivateCurrencyCommandHandler : ICommandHandler<ActivateCurrencyCommand, DataResult<Currency?>>
{
    private readonly IEntityService<Currency, Guid> _service;

    public ActivateCurrencyCommandHandler(
        IEntityService<Currency, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<Currency?>> ExecuteAsync(ActivateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.SetDeactivatedAsync(request.Id, false, cancellationToken);
        return DataResult<Currency?>.Success(result);
    }
}

public class ActivateCurrencyCommand : ICommand
{
    public Guid Id { get; set; }
}
