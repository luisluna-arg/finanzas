using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class DeleteFundOwnerRequest : BaseResourceOwnerRequest
{
    public DeleteFundOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}
