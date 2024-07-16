using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Debits;

public class ActivateDebitCommandHandler : IRequestHandler<ActivateDebitCommand, Debit?>
{
    private readonly IEntityService<Debit, Guid> service;

    public ActivateDebitCommandHandler(
        IEntityService<Debit, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Debit?> Handle(ActivateDebitCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateDebitCommand : IRequest<Debit?>
{
    public Guid Id { get; set; }
}
