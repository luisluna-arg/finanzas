using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Banks;

public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand>
{
    private readonly IEntityService<Bank, Guid> service;

    public DeleteBankCommandHandler(
        IEntityService<Bank, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteBankCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
