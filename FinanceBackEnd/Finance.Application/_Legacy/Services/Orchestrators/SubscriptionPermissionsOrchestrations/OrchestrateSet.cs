using CQRSDispatch;
using Finance.Application.Legacy.Commands;
using Finance.Application.Legacy.Queries.Subscriptions;
using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Legacy.Services.Orchestrators.SubscriptionPermissionsOrchestrations;

public sealed partial class SubscriptionPermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetSubscriptionOwnerSagaRequest, DataResult<SubscriptionPermissions>, DeleteSubscriptionOwnerSagaRequest, CommandResult>
{
    public override async Task<DataResult<SubscriptionPermissions>> OrchestrateSet(SetSubscriptionOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var ResourcePermissions = await Dispatcher.DispatchQueryAsync(new GetSubscriptionOwnershipQuery(request.Id), httpRequest);
        if (ResourcePermissions.IsSuccess && ResourcePermissions.Data?.Any() == true)
        {
            return DataResult<SubscriptionPermissions>.Success(ResourcePermissions.Data.First().ResourcePermissions);
        }

        var createSubscriptionPermissionsCommand = new CreateSubscriptionPermissionsCommand
        {
            ResourceId = request.Id,
            UserId = request.UserId,
            PermissionLevels = [PermissionLevelEnum.Owner]
        };
        var SubscriptionPermissionsResult = await Dispatcher.DispatchAsync(createSubscriptionPermissionsCommand, httpRequest);
        if (!SubscriptionPermissionsResult.IsSuccess || SubscriptionPermissionsResult.Data == null)
        {
            throw new Exception(SubscriptionPermissionsResult.ErrorMessage);
        }

        return DataResult<SubscriptionPermissions>.Success(SubscriptionPermissionsResult.Data);
    }
}
