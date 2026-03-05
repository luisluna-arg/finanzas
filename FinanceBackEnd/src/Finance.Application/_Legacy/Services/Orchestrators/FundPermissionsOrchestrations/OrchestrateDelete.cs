using CQRSDispatch;
using Finance.Application.Legacy.Commands;
using Finance.Application.Legacy.Commands.Funds.Owners;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Legacy.Services.Orchestrators.FundPermissionsOrchestrations;

public sealed partial class FundPermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetFundOwnerSagaRequest, DataResult<FundPermissions>, DeleteFundOwnerSagaRequest, CommandResult>
{
    public override async Task<CommandResult> OrchestrateDelete(DeleteFundOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var deleteFundOwnerCommand = new DeleteFundOwnerCommand
        {
            EntityId = request.Id
        };
        var createResourceResult = await Dispatcher.DispatchAsync(deleteFundOwnerCommand, httpRequest);

        if (!createResourceResult.IsSuccess)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        return CommandResult.Success();
    }
}
