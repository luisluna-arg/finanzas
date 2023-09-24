using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Banks;

public class ActivateBankCommandHandler : IRequestHandler<ActivateBankCommand, Bank?>
{
    private readonly IEntityService<Bank, Guid> service;

    public ActivateBankCommandHandler(
        IEntityService<Bank, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Bank?> Handle(ActivateBankCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, false);
}

public class ActivateBankCommand : IRequest<Bank?>
{
    public Guid Id { get; set; }
}
