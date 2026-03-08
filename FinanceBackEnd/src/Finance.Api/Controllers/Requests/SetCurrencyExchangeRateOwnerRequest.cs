using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class SetCurrencyExchangeRateOwnerRequest : BaseResourceOwnerRequest
{
    public SetCurrencyExchangeRateOwnerRequest(Guid resourceId, Guid userId)
        : base(resourceId, userId)
    {
    }
}
