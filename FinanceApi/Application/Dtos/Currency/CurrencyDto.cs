namespace FinanceApi.Application.Dtos.Currency;

public record CurrencyDto : Dto
{
    public string Name { get; set; } = string.Empty;
}
