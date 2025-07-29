using CQRSDispatch;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.Funds.Owners;

public class DeleteFundOwnerSagaRequest : BaseFundOwnerSagaRequest<CommandResult>, ISagaRequest
{
    public DeleteFundOwnerSagaRequest(Guid id) : base(id)
    {
    }
}
