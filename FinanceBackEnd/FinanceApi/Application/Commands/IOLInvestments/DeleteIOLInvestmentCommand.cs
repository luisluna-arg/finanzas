using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class DeleteIOLInvestmentCommandHandler : IRequestHandler<DeleteIOLInvestmentCommand>
{
    private readonly IEntityService<IOLInvestment, Guid> service;

    public DeleteIOLInvestmentCommandHandler(
        IEntityService<IOLInvestment, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteIOLInvestmentCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteIOLInvestmentCommand : IRequest
{
    public Guid[] Ids { get; set; }
}