using CQRSDispatch;
using Finance.Application.Legacy.Commands.Users;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Services.Requests.Subscriptions.Owners.Base;

public abstract class BaseSubscriptionOwnerSagaRequest<TResult> : OwnerBaseCommand<TResult>, ISagaRequest
    where TResult : RequestResult
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    protected BaseSubscriptionOwnerSagaRequest(Guid id)
    {
        Id = id;
    }

    protected BaseSubscriptionOwnerSagaRequest(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}
