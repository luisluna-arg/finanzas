using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetIOLInvestmentOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);

public sealed class DeleteIOLInvestmentOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);
