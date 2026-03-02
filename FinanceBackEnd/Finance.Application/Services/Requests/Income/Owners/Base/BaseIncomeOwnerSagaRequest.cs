using CQRSDispatch;
using Finance.Application.Commands.Users;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.Incomes.Owners.Base;

public abstract class BaseIncomeOwnerSagaRequest<TResult> : OwnerBaseCommand<TResult>, ISagaRequest
    where TResult : RequestResult
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    protected BaseIncomeOwnerSagaRequest(Guid id)
    {
        Id = id;
    }

    protected BaseIncomeOwnerSagaRequest(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}
