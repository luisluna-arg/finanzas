using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetIncomeOwnerRequest(Guid fundId, Guid userId)
    : BaseResourceOwnerRequest(fundId, userId);

public sealed class DeleteIncomeOwnerRequest(Guid fundId, Guid userId)
    : BaseResourceOwnerRequest(fundId, userId);
