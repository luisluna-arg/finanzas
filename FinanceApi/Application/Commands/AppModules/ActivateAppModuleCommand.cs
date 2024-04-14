using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.AppModules;

public class ActivateAppModuleCommandHandler : IRequestHandler<ActivateAppModuleCommand, AppModule?>
{
    private readonly IEntityService<AppModule, Guid> service;

    public ActivateAppModuleCommandHandler(
        IEntityService<AppModule, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<AppModule?> Handle(ActivateAppModuleCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateAppModuleCommand : IRequest<AppModule?>
{
    public Guid Id { get; set; }
}
