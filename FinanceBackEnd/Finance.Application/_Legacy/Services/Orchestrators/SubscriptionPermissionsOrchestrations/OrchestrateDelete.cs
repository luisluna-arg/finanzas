using CQRSDispatch;
using Finance.Application.Legacy.Commands;
using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Legacy.Services.Orchestrators.SubscriptionPermissionsOrchestrations;

public sealed partial class SubscriptionPermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetSubscriptionOwnerSagaRequest, DataResult<SubscriptionPermissions>, DeleteSubscriptionOwnerSagaRequest, CommandResult>
{
    public override async Task<CommandResult> OrchestrateDelete(DeleteSubscriptionOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var deleteSubscriptionOwnerCommand = new DeleteSubscriptionOwnerCommand
        {
            EntityId = request.Id
        };
        var createResourceResult = await Dispatcher.DispatchAsync(deleteSubscriptionOwnerCommand, httpRequest);

        if (!createResourceResult.IsSuccess)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        return CommandResult.Success();
    }
}
