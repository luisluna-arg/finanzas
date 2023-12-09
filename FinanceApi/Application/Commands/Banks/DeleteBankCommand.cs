using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Banks;

public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand>
{
    private readonly IEntityService<Bank, Guid> service;

    public DeleteBankCommandHandler(
        IEntityService<Bank, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        => await service.Delete(request.Ids);
}

public class DeleteBankCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
