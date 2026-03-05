using CQRSDispatch;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Legacy.Commands.CurrencyExchangeRates.Owners;

public class SetCurrencyExchangeRateOwnerSagaRequest : BaseCurrencyExchangeRateOwnerSagaRequest<DataResult<CurrencyExchangeRatePermissions>>, ISagaRequest
{
    public SetCurrencyExchangeRateOwnerSagaRequest(Guid id)
        : base(id)
    {
    }
}
