using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;

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
