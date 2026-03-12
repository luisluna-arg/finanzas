using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetIncomeOwnerRequest : BaseResourceOwnerRequest
{
    public SetIncomeOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}

public sealed class DeleteIncomeOwnerRequest : BaseResourceOwnerRequest
{
    public DeleteIncomeOwnerRequest(Guid fundId, Guid userId)
        : base(fundId, userId)
    {
    }
}
