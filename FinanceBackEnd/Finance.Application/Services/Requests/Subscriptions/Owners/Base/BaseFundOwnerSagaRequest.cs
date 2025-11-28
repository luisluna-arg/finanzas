using CQRSDispatch;
using Finance.Application.Commands.Users;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Subscriptions.Owners.Base;

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
