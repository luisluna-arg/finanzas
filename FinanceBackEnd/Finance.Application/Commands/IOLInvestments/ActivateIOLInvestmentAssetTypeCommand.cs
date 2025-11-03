using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetCommandHandler : ICommandHandler<ActivateIOLInvestmentAssetCommand, DataResult<IOLInvestmentAsset?>>
{
    private readonly IEntityService<IOLInvestmentAsset, Guid> _service;

    public ActivateIOLInvestmentAssetCommandHandler(
        IEntityService<IOLInvestmentAsset, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<IOLInvestmentAsset?>> ExecuteAsync(ActivateIOLInvestmentAssetCommand request, CancellationToken cancellationToken)
        => DataResult<IOLInvestmentAsset?>.Success(await _service.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateIOLInvestmentAssetCommand : ICommand<DataResult<IOLInvestmentAsset?>>
{
    public Guid Id { get; set; }
}
