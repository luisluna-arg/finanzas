using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetTypeCommandHandler : IRequestHandler<ActivateIOLInvestmentAssetTypeCommand, IOLInvestmentAssetType?>
{
    private readonly IEntityService<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> _iolInvestmentAssetTypeRepository;

    public ActivateIOLInvestmentAssetTypeCommandHandler(
        IEntityService<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> iolInvestmentAssetTypeRepository)
    {
        this._iolInvestmentAssetTypeRepository = iolInvestmentAssetTypeRepository;
    }

    public async Task<IOLInvestmentAssetType?> Handle(ActivateIOLInvestmentAssetTypeCommand request, CancellationToken cancellationToken)
        => await _iolInvestmentAssetTypeRepository.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateIOLInvestmentAssetTypeCommand : IRequest<IOLInvestmentAssetType?>
{
    public IOLInvestmentAssetTypeEnum Id { get; set; }
}
