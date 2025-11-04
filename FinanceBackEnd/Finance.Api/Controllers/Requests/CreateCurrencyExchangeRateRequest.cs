using Finance.Application.Commands.CurrencyExchangeRates;

namespace Finance.Api.Controllers.Requests;

public class CreateCurrencyExchangeRateRequest : CreateCurrencyExchangeRateCommand
{
    public CreateCurrencyExchangeRateRequest(Guid baseCurrencyId, Guid quoteCurrencyId, decimal buyRate, decimal sellRate, DateTime timeStamp)
    {
        BaseCurrencyId = baseCurrencyId;
        QuoteCurrencyId = quoteCurrencyId;
        BuyRate = buyRate;
        SellRate = sellRate;
        TimeStamp = timeStamp;
    }
}
