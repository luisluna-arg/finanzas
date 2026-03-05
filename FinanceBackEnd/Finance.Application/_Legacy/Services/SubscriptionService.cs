using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Orchestrators.SubscriptionOrchestrations;
using Finance.Application.Legacy.Services.Orchestrators.SubscriptionPermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Subscriptions;
using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Legacy.Services;

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
