using Finance.Api.Controllers.Requests.Base;

namespace Finance.Api.Controllers.Requests;

public class DeleteCurrencyExchangeRateOwnerRequest : BaseResourceOwnerRequest
{
    public DeleteCurrencyExchangeRateOwnerRequest(Guid resourceId, Guid userId)
        : base(resourceId, userId)
    {
    }
}
