using CQRSDispatch;
using Finance.Application.Legacy.Commands.Funds.Owners.Base;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Commands.Funds.Owners;

public class DeleteFundOwnerSagaRequest : BaseFundOwnerSagaRequest<CommandResult>, ISagaRequest
{
    public DeleteFundOwnerSagaRequest(Guid id) : base(id)
    {
    }
}
