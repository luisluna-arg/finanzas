using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.Currencies;

public class DeactivateCurrencyCommandHandler : ICommandHandler<DeactivateCurrencyCommand, DataResult<Currency?>>
{
    private readonly IEntityService<Currency, Guid> _service;

    public DeactivateCurrencyCommandHandler(
        IEntityService<Currency, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<Currency?>> ExecuteAsync(DeactivateCurrencyCommand request, CancellationToken cancellationToken)
        => DataResult<Currency?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateCurrencyCommand : ICommand<DataResult<Currency?>>
{
    public Guid Id { get; set; }
}
