using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class SetFundOwnerRequest : BaseFundOwnerRequest
{
    public SetFundOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}