using CQRSDispatch;
using Finance.Application.Commands.Users;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.Funds.Owners;

public abstract class BaseFundOwnerSagaRequest<TResult> : OwnerBaseCommand<TResult>, ISagaRequest
    where TResult : RequestResult
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public BaseFundOwnerSagaRequest(Guid id)
    {
        Id = id;
    }

    public BaseFundOwnerSagaRequest(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}
