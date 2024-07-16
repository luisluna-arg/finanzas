using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentAssetTypeCommandHandler : IRequestHandler<DeactivateIOLInvestmentAssetTypeCommand, IOLInvestmentAssetType?>
{
    private readonly IEntityService<IOLInvestmentAssetType, ushort> service;

    public DeactivateIOLInvestmentAssetTypeCommandHandler(
        IEntityService<IOLInvestmentAssetType, ushort> repository)
    {
        this.service = repository;
    }

    public async Task<IOLInvestmentAssetType?> Handle(DeactivateIOLInvestmentAssetTypeCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateIOLInvestmentAssetTypeCommand : IRequest<IOLInvestmentAssetType?>
{
    public ushort Id { get; set; }
}
