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
    public override async Task<DataResult<Subscription>> OrchestrateCreation(CreateSubscriptionSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new CreateSubscriptionCommand()
        {
            Name = request.Name,
            Price = request.Price,
            Frequency = request.Frequency,
            CurrencyId = request.CurrencyId,
            UserId = request.UserId
        };

        var createSubscriptionResult = await Dispatcher.DispatchAsync(command, httpRequest);
        if (!createSubscriptionResult.IsSuccess)
        {
            throw new Exception(createSubscriptionResult.ErrorMessage);
        }

        var SubscriptionPermissionsOwnerResult = await OwnerService.Set(
            new SetSubscriptionOwnerSagaRequest(createSubscriptionResult.Data.Id),
            transaction: transaction,
            httpRequest: httpRequest);

        if (!SubscriptionPermissionsOwnerResult.IsSuccess)
        {
            throw new Exception(SubscriptionPermissionsOwnerResult.ErrorMessage);
        }

        return createSubscriptionResult;
    }
}
