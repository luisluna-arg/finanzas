using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetFundOwnerRequest : BaseResourceOwnerRequest
{
    public SetFundOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}

public sealed class DeleteFundOwnerRequest : BaseResourceOwnerRequest
{
    public DeleteFundOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}
