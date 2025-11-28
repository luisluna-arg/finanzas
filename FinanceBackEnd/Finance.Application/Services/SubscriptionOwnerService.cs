using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators.SubscriptionPermissionsOrchestrations;
using Finance.Application.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Services;

public class SubscriptionOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : BaseResourcePermissionsSagaService<
        SubscriptionPermissions,
        SubscriptionPermissionsOrchestrator,
        SetSubscriptionOwnerSagaRequest,
        DataResult<SubscriptionPermissions>,
        DeleteSubscriptionOwnerSagaRequest,
        CommandResult>(dispatcher, dbContext);
