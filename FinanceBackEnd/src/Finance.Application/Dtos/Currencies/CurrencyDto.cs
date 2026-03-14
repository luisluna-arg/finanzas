using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.Currencies;

public record CurrencyDto : CatalogDto<Guid>
{
    public CurrencyDto()
        : base()
    {
    }

    public string ShortName { get; set; } = string.Empty;
}
