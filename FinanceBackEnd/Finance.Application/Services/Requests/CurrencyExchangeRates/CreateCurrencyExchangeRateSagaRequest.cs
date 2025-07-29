using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.CurrencyExchangeRates;

public class CreateCurrencyExchangeRateSagaRequest : CreateCurrencyExchangeRateCommand, ISagaRequest
{
    public CreateCurrencyExchangeRateSagaRequest(Guid baseCurrencyId, Guid quoteCurrencyId, decimal buyRate, decimal sellRate, DateTime timeStamp)
        : base()
    {
        BaseCurrencyId = baseCurrencyId;
        QuoteCurrencyId = quoteCurrencyId;
        BuyRate = buyRate;
        SellRate = sellRate;
        TimeStamp = timeStamp;
    }
}
