using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetCommandHandler : IRequestHandler<ActivateIOLInvestmentAssetCommand, IOLInvestmentAsset?>
{
    private readonly IEntityService<IOLInvestmentAsset, Guid> service;

    public ActivateIOLInvestmentAssetCommandHandler(
        IEntityService<IOLInvestmentAsset, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<IOLInvestmentAsset?> Handle(ActivateIOLInvestmentAssetCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateIOLInvestmentAssetCommand : IRequest<IOLInvestmentAsset?>
{
    public Guid Id { get; set; }
}
