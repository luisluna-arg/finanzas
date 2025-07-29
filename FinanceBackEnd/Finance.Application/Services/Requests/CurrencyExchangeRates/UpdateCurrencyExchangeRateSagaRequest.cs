using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.CurrencyExchangeRates;

public class UpdateCurrencyExchangeRateSagaRequest : UpdateCurrencyExchangeRateCommand, ISagaRequest
{
    public UpdateCurrencyExchangeRateSagaRequest(Guid id, decimal buyRate, decimal sellRate)
        : base()
    {
        Id = id;
        BuyRate = buyRate;
        SellRate = sellRate;
    }
}
