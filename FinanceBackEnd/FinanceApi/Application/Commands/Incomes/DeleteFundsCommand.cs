using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Incomes;

public class DeleteIncomesCommandHandler : IRequestHandler<DeleteIncomesCommand>
{
    private readonly IEntityService<Income, Guid> service;

    public DeleteIncomesCommandHandler(
        IEntityService<Income, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteIncomesCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteIncomesCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
