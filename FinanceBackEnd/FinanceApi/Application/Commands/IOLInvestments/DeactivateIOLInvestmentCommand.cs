using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentCommandHandler : IRequestHandler<DeactivateIOLInvestmentCommand, IOLInvestment?>
{
    private readonly IEntityService<IOLInvestment, Guid> service;

    public DeactivateIOLInvestmentCommandHandler(
        IEntityService<IOLInvestment, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<IOLInvestment?> Handle(DeactivateIOLInvestmentCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateIOLInvestmentCommand : IRequest<IOLInvestment?>
{
    public Guid Id { get; set; }
}
