using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Banks;

public class DeactivateBankCommandHandler : IRequestHandler<DeactivateBankCommand, Bank?>
{
    private readonly IEntityService<Bank, Guid> service;

    public DeactivateBankCommandHandler(
        IEntityService<Bank, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Bank?> Handle(DeactivateBankCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, true);
}

public class DeactivateBankCommand : IRequest<Bank?>
{
    public Guid Id { get; set; }
}
