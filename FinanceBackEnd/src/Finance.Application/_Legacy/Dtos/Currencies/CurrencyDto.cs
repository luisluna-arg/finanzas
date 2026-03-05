using Finance.Application.Legacy.Dtos.Base;

namespace Finance.Application.Legacy.Dtos.Currencies;

public record CurrencyDto : CatalogDto<Guid>
{
    public CurrencyDto()
        : base()
    {
    }

    public string ShortName { get; set; } = string.Empty;
}
