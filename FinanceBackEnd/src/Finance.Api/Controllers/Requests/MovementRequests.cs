using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetMovementOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);

public sealed class DeleteMovementOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);
