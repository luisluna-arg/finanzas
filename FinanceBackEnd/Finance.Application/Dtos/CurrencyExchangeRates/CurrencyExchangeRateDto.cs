using Finance.Application.Dtos.Currencies;

namespace Finance.Application.Dtos;

public record CurrencyExchangeRateDto : Dto<Guid>
{
    public CurrencyExchangeRateDto()
        : base()
    {
    }

    public CurrencyDto BaseCurrency { get; set; }

    public CurrencyDto QuoteCurrency { get; set; }

    public decimal BuyRate { get; set; }

    public decimal SellRate { get; set; }

    public DateTime TimeStamp { get; set; }
}
