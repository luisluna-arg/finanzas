using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetFundOwnerRequest(Guid fundId, Guid userId)
    : BaseResourceOwnerRequest(fundId, userId);

public sealed class DeleteFundOwnerRequest(Guid fundId, Guid userId)
    : BaseResourceOwnerRequest(fundId, userId);
