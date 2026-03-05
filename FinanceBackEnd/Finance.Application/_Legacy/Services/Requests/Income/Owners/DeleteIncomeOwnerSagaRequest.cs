using CQRSDispatch;
using Finance.Application.Legacy.Commands.Incomes.Owners.Base;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Commands.Incomes.Owners;

public class DeleteIncomeOwnerSagaRequest : BaseIncomeOwnerSagaRequest<CommandResult>, ISagaRequest
{
    public DeleteIncomeOwnerSagaRequest(Guid id) : base(id)
    {
    }
}
