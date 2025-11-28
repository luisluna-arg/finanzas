using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators.SubscriptionPermissionsOrchestrations;

public sealed partial class SubscriptionPermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetSubscriptionOwnerSagaRequest, DataResult<SubscriptionPermissions>, DeleteSubscriptionOwnerSagaRequest, CommandResult>
{
    public override async Task<CommandResult> OrchestrateDelete(DeleteSubscriptionOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var deleteSubscriptionOwnerCommand = new DeleteSubscriptionOwnerCommand
        {
            EntityId = request.Id
        };
        var createResourceResult = await Dispatcher.DispatchAsync(deleteSubscriptionOwnerCommand);

        if (!createResourceResult.IsSuccess)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        return CommandResult.Success();
    }
}
