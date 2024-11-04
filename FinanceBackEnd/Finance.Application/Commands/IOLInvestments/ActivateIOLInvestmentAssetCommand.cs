using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetTypeCommandHandler : IRequestHandler<ActivateIOLInvestmentAssetTypeCommand, IOLInvestmentAssetType?>
{
    private readonly IEntityService<IOLInvestmentAssetType, ushort> service;

    public ActivateIOLInvestmentAssetTypeCommandHandler(
        IEntityService<IOLInvestmentAssetType, ushort> repository)
    {
        this.service = repository;
    }

    public async Task<IOLInvestmentAssetType?> Handle(ActivateIOLInvestmentAssetTypeCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateIOLInvestmentAssetTypeCommand : IRequest<IOLInvestmentAssetType?>
{
    public ushort Id { get; set; }
}
