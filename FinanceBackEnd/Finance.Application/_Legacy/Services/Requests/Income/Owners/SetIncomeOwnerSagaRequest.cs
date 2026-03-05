using CQRSDispatch;
using Finance.Application.Legacy.Commands.Incomes.Owners.Base;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Legacy.Commands.Incomes.Owners;

public class SetIncomeOwnerSagaRequest : BaseIncomeOwnerSagaRequest<DataResult<IncomePermissions>>, ISagaRequest
{
    public SetIncomeOwnerSagaRequest(Guid id) : base(id)
    {
    }

    public SetIncomeOwnerSagaRequest(Guid id, Guid userId) : base(id, userId)
    {
    }
}
