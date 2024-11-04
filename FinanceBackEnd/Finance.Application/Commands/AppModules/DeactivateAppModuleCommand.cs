using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.AppModules;

public class DeactivateAppModuleCommandHandler : IRequestHandler<DeactivateAppModuleCommand, AppModule?>
{
    private readonly IEntityService<AppModule, Guid> service;

    public DeactivateAppModuleCommandHandler(
        IEntityService<AppModule, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<AppModule?> Handle(DeactivateAppModuleCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateAppModuleCommand : IRequest<AppModule?>
{
    public Guid Id { get; set; }
}
