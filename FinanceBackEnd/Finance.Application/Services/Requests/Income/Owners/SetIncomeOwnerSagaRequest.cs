using CQRSDispatch;
using Finance.Application.Commands.Incomes.Owners.Base;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Commands.Incomes.Owners;

public class SetIncomeOwnerSagaRequest : BaseIncomeOwnerSagaRequest<DataResult<IncomePermissions>>, ISagaRequest
{
    public SetIncomeOwnerSagaRequest(Guid id) : base(id)
    {
    }

    public SetIncomeOwnerSagaRequest(Guid id, Guid userId) : base(id, userId)
    {
    }
}
