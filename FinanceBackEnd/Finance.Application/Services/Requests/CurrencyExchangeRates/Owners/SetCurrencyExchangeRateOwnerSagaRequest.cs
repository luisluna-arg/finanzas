using CQRSDispatch;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models;

namespace Finance.Application.Commands.CurrencyExchangeRates.Owners;

public class SetCurrencyExchangeRateOwnerSagaRequest : BaseCurrencyExchangeRateOwnerSagaRequest<DataResult<CurrencyExchangeRateResource>>, ISagaRequest
{
    public SetCurrencyExchangeRateOwnerSagaRequest(Guid id)
        : base(id)
    {
    }
}
