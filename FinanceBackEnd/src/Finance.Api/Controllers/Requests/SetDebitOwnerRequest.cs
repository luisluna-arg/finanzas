using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class SetDebitOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);
