using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetSubscriptionOwnerRequest(Guid subscriptionId, Guid userId)
    : BaseResourceOwnerRequest(subscriptionId, userId);

public sealed class DeleteSubscriptionOwnerRequest(Guid subscriptionId, Guid userId)
    : BaseResourceOwnerRequest(subscriptionId, userId);
