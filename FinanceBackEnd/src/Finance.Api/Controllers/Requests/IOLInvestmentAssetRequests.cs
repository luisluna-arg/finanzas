using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetIOLInvestmentAssetOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);

public sealed class DeleteIOLInvestmentAssetOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);
