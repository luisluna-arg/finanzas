using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services.Base;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Orchestrators.SubscriptionOrchestrations;
using Finance.Application.Services.Orchestrators.SubscriptionPermissionsOrchestrations;
using Finance.Application.Services.Requests.Subscriptions;
using Finance.Application.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Services;

public class SubscriptionService
    : BaseResourceSagaService<
        Subscription,
        SubscriptionPermissions,
        SubscriptionOrchestrator,
        SubscriptionPermissionsOrchestrator,
        CreateSubscriptionSagaRequest,
        UpdateSubscriptionSagaRequest,
        DeleteSubscriptionSagaRequest,
        SetSubscriptionOwnerSagaRequest,
        DataResult<SubscriptionPermissions>,
        DeleteSubscriptionOwnerSagaRequest,
        CommandResult>
{
    public SubscriptionService(
        IDispatcher<FinanceDispatchContext> dispatcher,
        FinanceDbContext dbContext,
        IResourcePermissionsSagaService<
            SubscriptionPermissions,
            SubscriptionPermissionsOrchestrator,
            SetSubscriptionOwnerSagaRequest,
            DataResult<SubscriptionPermissions>,
            DeleteSubscriptionOwnerSagaRequest,
            CommandResult> ownerService)
        : base(dispatcher, dbContext, ownerService)
    {
    }
}
