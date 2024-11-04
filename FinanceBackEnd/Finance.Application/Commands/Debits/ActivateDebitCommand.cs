using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Debits;

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
