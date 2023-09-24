using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetCommandHandler : IRequestHandler<ActivateIOLInvestmentAssetCommand, IOLInvestmentAsset?>
{
    private readonly IEntityService<IOLInvestmentAsset, Guid> service;

    public ActivateIOLInvestmentAssetCommandHandler(
        IEntityService<IOLInvestmentAsset, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<IOLInvestmentAsset?> Handle(ActivateIOLInvestmentAssetCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, false);
}

public class ActivateIOLInvestmentAssetCommand : IRequest<IOLInvestmentAsset?>
{
    public Guid Id { get; set; }
}
