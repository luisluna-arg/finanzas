using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.AppModules;

namespace Finance.Application.Commands.AppModules;

public class ActivateAppModuleCommandHandler : ICommandHandler<ActivateAppModuleCommand, DataResult<AppModule?>>
{
    private readonly IEntityService<AppModule, Guid> _service;

    public ActivateAppModuleCommandHandler(
        IEntityService<AppModule, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<AppModule?>> ExecuteAsync(ActivateAppModuleCommand request, CancellationToken cancellationToken)
    {
        var aa = await _service.SetDeactivatedAsync(request.Id, false, cancellationToken);
        if (aa != null)
        {
            return new DataResult<AppModule?>(true, aa, "App module activated successfully");
        }
        else
        {
            return DataResult<AppModule?>.Failure("App module not found or already active");
        }
    }
}

public class ActivateAppModuleCommand : ICommand
{
    public Guid Id { get; set; }
}
