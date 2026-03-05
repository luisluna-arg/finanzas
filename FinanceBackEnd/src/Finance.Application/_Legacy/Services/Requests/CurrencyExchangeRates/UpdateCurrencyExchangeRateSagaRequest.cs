using Finance.Application.Legacy.Commands.CurrencyExchangeRates;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Services.Requests.CurrencyExchangeRates;

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
