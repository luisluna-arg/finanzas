using CQRSDispatch;
using Finance.Application.Commands.Users;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.FundOwners;

public abstract class BaseFundOwnerSagaRequest<TResult> : OwnerBaseCommand<TResult>, ISagaRequest
    where TResult : RequestResult
{
    public BaseFundOwnerSagaRequest(Guid userId, Guid fundId)
    {
        UserId = userId;
        FundId = fundId;
    }

    public Guid FundId { get; set; }
}
