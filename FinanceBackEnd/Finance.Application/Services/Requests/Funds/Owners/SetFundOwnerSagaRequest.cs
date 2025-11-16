using CQRSDispatch;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Commands.Funds.Owners;

public class SetFundOwnerSagaRequest : BaseFundOwnerSagaRequest<DataResult<FundPermissions>>, ISagaRequest
{
    public SetFundOwnerSagaRequest(Guid id) : base(id)
    {
    }

    public SetFundOwnerSagaRequest(Guid id, Guid userId) : base(id, userId)
    {
    }
}
