using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Legacy.Commands.Incomes.Owners;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Legacy.Services.Orchestrators.IncomePermissionsOrchestrations;

public sealed partial class IncomePermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetIncomeOwnerSagaRequest, DataResult<IncomePermissions>, DeleteIncomeOwnerSagaRequest, CommandResult>
{
    public override async Task<CommandResult> OrchestrateDelete(DeleteIncomeOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var deleteIncomeOwnerCommand = new DeleteIncomeOwnerCommand
        {
            EntityId = request.Id
        };
        var createResourceResult = await Dispatcher.DispatchAsync(deleteIncomeOwnerCommand, httpRequest);

        if (!createResourceResult.IsSuccess)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        return CommandResult.Success();
    }
}
