using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class SetIncomeOwnerRequest : BaseResourceOwnerRequest
{
    public SetIncomeOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}
