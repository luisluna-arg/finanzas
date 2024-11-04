using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.IOLInvestments;

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
