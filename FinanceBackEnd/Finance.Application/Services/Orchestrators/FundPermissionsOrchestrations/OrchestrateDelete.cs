using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds.Owners;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators.FundPermissionsOrchestrations;

public sealed partial class FundPermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetFundOwnerSagaRequest, DataResult<FundPermissions>, DeleteFundOwnerSagaRequest, CommandResult>
{
    public override async Task<CommandResult> OrchestrateDelete(DeleteFundOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var deleteFundOwnerCommand = new DeleteFundOwnerCommand
        {
            EntityId = request.Id
        };
        var createResourceResult = await Dispatcher.DispatchAsync(deleteFundOwnerCommand);

        if (!createResourceResult.IsSuccess)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        return CommandResult.Success();
    }
}
