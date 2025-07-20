using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.AppModules;

public class DeactivateAppModuleCommandHandler : ICommandHandler<DeactivateAppModuleCommand, DataResult<AppModule?>>
{
    private readonly IEntityService<AppModule, Guid> _service;

    public DeactivateAppModuleCommandHandler(
        IEntityService<AppModule, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<AppModule?>> ExecuteAsync(DeactivateAppModuleCommand request, CancellationToken cancellationToken)
    {
        await _service.SetDeactivatedAsync(request.Id, true, cancellationToken);
        return DataResult<AppModule?>.Success();
    }
}

public class DeactivateAppModuleCommand : ICommand<DataResult<AppModule?>>
{
    public Guid Id { get; set; }
}
