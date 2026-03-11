using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class DeleteFundOwnerRequest : BaseResourceOwnerRequest
{
    public DeleteFundOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}
