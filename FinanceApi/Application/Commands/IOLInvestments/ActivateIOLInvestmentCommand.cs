using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentCommandHandler : IRequestHandler<ActivateIOLInvestmentCommand, IOLInvestment?>
{
    private readonly IEntityService<IOLInvestment, Guid> service;

    public ActivateIOLInvestmentCommandHandler(
        IEntityService<IOLInvestment, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<IOLInvestment?> Handle(ActivateIOLInvestmentCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, false);
}

public class ActivateIOLInvestmentCommand : IRequest<IOLInvestment?>
{
    public Guid Id { get; set; }
}
