using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class SetSubscriptionOwnerRequest : BaseResourceOwnerRequest
{
    public SetSubscriptionOwnerRequest(Guid subscriptionId, Guid userId)
        : base(subscriptionId, userId)
    {
    }
}
