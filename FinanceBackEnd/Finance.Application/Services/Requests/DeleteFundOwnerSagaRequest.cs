using CQRSDispatch;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands;

public class DeleteFundOwnerSagaRequest : BaseFundOwnerSagaRequest<CommandResult>, ISagaRequest
{
    public DeleteFundOwnerSagaRequest(Guid userId, Guid fundId) : base(userId, fundId)
    {
    }
}
