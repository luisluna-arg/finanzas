using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class DeleteSubscriptionOwnerRequest(Guid subscriptionId, Guid userId)
    : BaseResourceOwnerRequest(subscriptionId, userId);
