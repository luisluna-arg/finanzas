namespace FinanceApi.Application.Dtos.Currency;

public record CurrencyDto : Dto
{
    public CurrencyDto() : base()
    {
    }

    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
}
