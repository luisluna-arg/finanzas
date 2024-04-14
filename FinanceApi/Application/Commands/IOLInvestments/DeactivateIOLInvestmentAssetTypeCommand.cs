using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentAssetCommandHandler : IRequestHandler<DeactivateIOLInvestmentAssetCommand, IOLInvestmentAsset?>
{
    private readonly IEntityService<IOLInvestmentAsset, Guid> service;

    public DeactivateIOLInvestmentAssetCommandHandler(
        IEntityService<IOLInvestmentAsset, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<IOLInvestmentAsset?> Handle(DeactivateIOLInvestmentAssetCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateIOLInvestmentAssetCommand : IRequest<IOLInvestmentAsset?>
{
    public Guid Id { get; set; }
}
