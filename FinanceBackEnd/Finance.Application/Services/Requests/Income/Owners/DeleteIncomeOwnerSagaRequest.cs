using CQRSDispatch;
using Finance.Application.Commands.Incomes.Owners.Base;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.Incomes.Owners;

public class DeleteIncomeOwnerSagaRequest : BaseIncomeOwnerSagaRequest<CommandResult>, ISagaRequest
{
    public DeleteIncomeOwnerSagaRequest(Guid id) : base(id)
    {
    }
}
