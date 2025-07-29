using CQRSDispatch;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models;

namespace Finance.Application.Commands.Funds.Owners;

public class SetFundOwnerSagaRequest : BaseFundOwnerSagaRequest<DataResult<FundResource>>, ISagaRequest
{
    public SetFundOwnerSagaRequest(Guid id) : base(id)
    {
    }
}
