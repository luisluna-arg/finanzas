using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Debits;

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
    public Guid[] Ids { get; set; }
}
