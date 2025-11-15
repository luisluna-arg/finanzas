using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentAssetCommandHandler : ICommandHandler<DeactivateIOLInvestmentAssetCommand, DataResult<IOLInvestmentAsset?>>
{
    private readonly IEntityService<IOLInvestmentAsset, Guid> _service;

    public DeactivateIOLInvestmentAssetCommandHandler(
        IEntityService<IOLInvestmentAsset, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<IOLInvestmentAsset?>> ExecuteAsync(DeactivateIOLInvestmentAssetCommand request, CancellationToken cancellationToken)
        => DataResult<IOLInvestmentAsset?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateIOLInvestmentAssetCommand : ICommand<DataResult<IOLInvestmentAsset?>>
{
    public Guid Id { get; set; }
}
