using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class DeleteSubscriptionOwnerRequest(Guid subscriptionId, Guid userId)
    : BaseResourceOwnerRequest(subscriptionId, userId);
