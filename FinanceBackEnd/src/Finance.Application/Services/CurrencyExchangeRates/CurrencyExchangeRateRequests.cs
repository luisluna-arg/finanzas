namespace Finance.Application.Services.CurrencyExchangeRates;

public sealed record CreateCurrencyExchangeRateRequest(
    Guid BaseCurrencyId,
    Guid QuoteCurrencyId,
    decimal BuyRate,
    decimal SellRate,
    DateTime TimeStamp);

public sealed record UpdateCurrencyExchangeRateRequest(
    Guid Id,
    decimal BuyRate,
    decimal SellRate);

public sealed record DeleteCurrencyExchangeRateRequest(Guid[] Ids);
