using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Debits;

public class DeleteDebitCommandHandler : IRequestHandler<DeleteDebitCommand>
{
    private readonly IEntityService<Debit, Guid> service;

    public DeleteDebitCommandHandler(
        IEntityService<Debit, Guid> service)
    {
        this.service = service;
    }

    public async Task Handle(DeleteDebitCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteDebitCommand : IRequest
{
    public Guid[] Ids { get; set; } = [];
}
