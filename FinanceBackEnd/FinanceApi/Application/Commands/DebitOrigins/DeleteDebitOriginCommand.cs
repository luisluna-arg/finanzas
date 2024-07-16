using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.DebitOrigins;

public class DeleteDebitOriginCommandHandler : IRequestHandler<DeleteDebitOriginCommand>
{
    private readonly IEntityService<DebitOrigin, Guid> service;

    public DeleteDebitOriginCommandHandler(
        IEntityService<DebitOrigin, Guid> service)
    {
        this.service = service;
    }

    public async Task Handle(DeleteDebitOriginCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteDebitOriginCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
