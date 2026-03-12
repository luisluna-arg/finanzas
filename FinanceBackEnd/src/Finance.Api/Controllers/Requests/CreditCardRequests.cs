using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public sealed class SetCreditCardOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);

public sealed class DeleteCreditCardOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);
