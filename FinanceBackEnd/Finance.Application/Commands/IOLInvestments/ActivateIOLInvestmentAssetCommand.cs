using Finance.Domain.Enums;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetTypeCommandHandler : ICommandHandler<ActivateIOLInvestmentAssetTypeCommand, DataResult<IOLInvestmentAssetType?>>
{
    private readonly IEntityService<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> _iolInvestmentAssetTypeRepository;

    public ActivateIOLInvestmentAssetTypeCommandHandler(
        IEntityService<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> iolInvestmentAssetTypeRepository)
    {
        this._iolInvestmentAssetTypeRepository = iolInvestmentAssetTypeRepository;
    }

    public async Task<DataResult<IOLInvestmentAssetType?>> ExecuteAsync(ActivateIOLInvestmentAssetTypeCommand request, CancellationToken cancellationToken)
        => DataResult<IOLInvestmentAssetType?>.Success(
            await _iolInvestmentAssetTypeRepository.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateIOLInvestmentAssetTypeCommand : ICommand<DataResult<IOLInvestmentAssetType?>>
{
    public IOLInvestmentAssetTypeEnum Id { get; set; }
}
