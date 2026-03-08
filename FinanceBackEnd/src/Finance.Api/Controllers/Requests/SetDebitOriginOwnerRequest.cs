using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class SetDebitOriginOwnerRequest(Guid resourceId, Guid userId)
    : BaseResourceOwnerRequest(resourceId, userId);
