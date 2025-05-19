using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetTypeCommandHandler : IRequestHandler<ActivateIOLInvestmentAssetTypeCommand, IOLInvestmentAssetType?>
{
    private readonly IEntityService<IOLInvestmentAssetType, ushort> _iolInvestmentAssetTypeRepository;

    public ActivateIOLInvestmentAssetTypeCommandHandler(
        IEntityService<IOLInvestmentAssetType, ushort> iolInvestmentAssetTypeRepository)
    {
        this._iolInvestmentAssetTypeRepository = iolInvestmentAssetTypeRepository;
    }

    public async Task<IOLInvestmentAssetType?> Handle(ActivateIOLInvestmentAssetTypeCommand request, CancellationToken cancellationToken)
        => await _iolInvestmentAssetTypeRepository.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateIOLInvestmentAssetTypeCommand : IRequest<IOLInvestmentAssetType?>
{
    public ushort Id { get; set; }
}
