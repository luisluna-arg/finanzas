using Finance.Application.Dtos.Currencies;

namespace Finance.Application.Dtos;

public record CurrencyExchangeRateDto() : Dto<Guid>
{
    public CurrencyDto BaseCurrency { get; set; } = default!;

    public CurrencyDto QuoteCurrency { get; set; } = default!;

    public decimal BuyRate { get; set; }

    public decimal SellRate { get; set; }

    public DateTime TimeStamp { get; set; }
}
