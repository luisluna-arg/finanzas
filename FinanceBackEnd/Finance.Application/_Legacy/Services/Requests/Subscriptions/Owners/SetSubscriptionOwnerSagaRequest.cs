using CQRSDispatch;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners.Base;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Legacy.Services.Requests.Subscriptions.Owners;

public class SetSubscriptionOwnerSagaRequest : BaseSubscriptionOwnerSagaRequest<DataResult<SubscriptionPermissions>>, ISagaRequest
{
    public SetSubscriptionOwnerSagaRequest(Guid id) : base(id)
    {
    }

    public SetSubscriptionOwnerSagaRequest(Guid id, Guid userId) : base(id, userId)
    {
    }
}
