namespace Finance.Application.Dtos.Currencies;

public record CurrencyDto() : Dto<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty;
}
