using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Funds;

public class DeleteFundsCommandHandler : IRequestHandler<DeleteFundsCommand>
{
    private readonly IEntityService<Fund, Guid> service;

    public DeleteFundsCommandHandler(
        IEntityService<Fund, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteFundsCommand request, CancellationToken cancellationToken)
        => await service.Delete(request.Ids);
}

public class DeleteFundsCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
