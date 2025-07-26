using CQRSDispatch;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models;

namespace Finance.Application.Commands.FundOwners;

public class SetFundOwnerSagaRequest : BaseFundOwnerSagaRequest<DataResult<FundResource>>, ISagaRequest
{
    public SetFundOwnerSagaRequest(Guid fundId) : base(fundId)
    {
    }
}
