using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Debits;

public class DeactivateDebitCommandHandler : IRequestHandler<DeactivateDebitCommand, Debit?>
{
    private readonly IEntityService<Debit, Guid> service;

    public DeactivateDebitCommandHandler(
        IEntityService<Debit, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Debit?> Handle(DeactivateDebitCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateDebitCommand : IRequest<Debit?>
{
    public Guid Id { get; set; }
}
