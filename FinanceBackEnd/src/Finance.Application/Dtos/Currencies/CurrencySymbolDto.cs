using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.Currencies;

public record CurrencySymbolDto : Dto<Guid>
{
    public CurrencySymbolDto() { }

    public Guid CurrencyId { get; set; }
    public string Symbol { get; set; } = string.Empty;
}
