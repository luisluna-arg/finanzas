using Finance.Application.Legacy.Dtos.Base;
using Finance.Application.Legacy.Dtos.Currencies;

namespace Finance.Application.Legacy.Dtos;

public record CurrencyExchangeRateDto : Dto<Guid>
{
    public CurrencyExchangeRateDto() { }

    public CurrencyDto BaseCurrency { get; set; } = default!;
    public CurrencyDto QuoteCurrency { get; set; } = default!;
    public decimal BuyRate { get; set; } = 0M;
    public decimal SellRate { get; set; } = 0M;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}
