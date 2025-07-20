using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.Currencies;

namespace Finance.Application.Dtos;

public record CurrencyExchangeRateDto : Dto<Guid>
{
    public CurrencyDto BaseCurrency { get; set; } = default!;

    public CurrencyDto QuoteCurrency { get; set; } = default!;

    public decimal BuyRate { get; set; } = 0M;

    public decimal SellRate { get; set; } = 0M;

    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    public CurrencyExchangeRateDto()
    {
    }
}
