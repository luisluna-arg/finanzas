using Finance.Domain.Enums;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentAssetTypeCommandHandler : ICommandHandler<DeactivateIOLInvestmentAssetTypeCommand, DataResult<IOLInvestmentAssetType?>>
{
    private readonly IEntityService<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> _service;

    public DeactivateIOLInvestmentAssetTypeCommandHandler(
        IEntityService<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> service)
    {
        _service = service;
    }

    public async Task<DataResult<IOLInvestmentAssetType?>> ExecuteAsync(DeactivateIOLInvestmentAssetTypeCommand request, CancellationToken cancellationToken)
        => DataResult<IOLInvestmentAssetType?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateIOLInvestmentAssetTypeCommand : ICommand<DataResult<IOLInvestmentAssetType?>>
{
    public IOLInvestmentAssetTypeEnum Id { get; set; }
}
