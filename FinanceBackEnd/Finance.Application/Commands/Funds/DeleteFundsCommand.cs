using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Funds;

public class DeleteFundsCommandHandler : IRequestHandler<DeleteFundsCommand>
{
    private readonly IEntityService<Fund, Guid> service;

    public DeleteFundsCommandHandler(
        IEntityService<Fund, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteFundsCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteFundsCommand : IRequest
{
    public Guid[] Ids { get; set; } = [];
}
