using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.Currencies;

public record CurrencyDto : Dto<Guid>
{
    public CurrencyDto() { }

    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public ICollection<CurrencySymbolDto> Symbols { get; set; } = [];
    public string? DefaultSymbol => Symbols.FirstOrDefault()?.Symbol;
}
