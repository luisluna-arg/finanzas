using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.AppModules;

public class DeactivateAppModuleCommandHandler : IRequestHandler<DeactivateAppModuleCommand, AppModule?>
{
    private readonly IEntityService<AppModule, Guid> service;

    public DeactivateAppModuleCommandHandler(
        IEntityService<AppModule, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<AppModule?> Handle(DeactivateAppModuleCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, true);
}

public class DeactivateAppModuleCommand : IRequest<AppModule?>
{
    public Guid Id { get; set; }
}
