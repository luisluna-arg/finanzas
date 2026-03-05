using CQRSDispatch;
using Finance.Application.Legacy.Commands.Subscriptions;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Orchestrators.SubscriptionPermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Subscriptions;
using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Legacy.Services.Orchestrators.SubscriptionOrchestrations;

public partial class SubscriptionOrchestrator
    : BaseResourceOrchestrator<
        Subscription,
        SubscriptionPermissions,
        CreateSubscriptionSagaRequest,
        UpdateSubscriptionSagaRequest,
        DeleteSubscriptionSagaRequest,
        SetSubscriptionOwnerSagaRequest,
        DataResult<SubscriptionPermissions>,
        DeleteSubscriptionOwnerSagaRequest,
        CommandResult,
        SubscriptionPermissionsOrchestrator>
{
    public override async Task<CommandResult> OrchestrateDelete(DeleteSubscriptionSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new DeleteSubscriptionCommand
        {
            Ids = request.Ids
        };

        var result = await Dispatcher.DispatchAsync(command, httpRequest);

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorMessage);
        }

        return CommandResult.Success();
    }
}
