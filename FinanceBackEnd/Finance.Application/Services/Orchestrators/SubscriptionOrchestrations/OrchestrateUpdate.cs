using CQRSDispatch;
using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators.SubscriptionPermissionsOrchestrations;
using Finance.Application.Services.Requests.Subscriptions;
using Finance.Application.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Orchestrators.SubscriptionOrchestrations;

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
    public override async Task<DataResult<Subscription>> OrchestrateUpdate(UpdateSubscriptionSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new UpdateSubscriptionCommand
        {
            Id = request.SubscriptionId,
            Name = request.Name,
            Price = request.Price,
            CurrencyId = request.CurrencyId,
            Frequency = request.Frequency,
            UserId = request.UserId,
        };

        var result = await Dispatcher.DispatchAsync(command, httpRequest);

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorMessage);
        }

        return result!;
    }
}
