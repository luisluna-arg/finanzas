using CQRSDispatch;
using Finance.Application.Legacy.Commands.Users;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Commands.Incomes.Owners.Base;

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
