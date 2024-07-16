namespace FinanceApi.Application.Dtos.CurrencyConversions;

public record CurrencyConversionDto : Dto<Guid>
{
    public CurrencyConversionDto()
        : base()
    {
    }

    public Guid MovementId { get; set; }

    public decimal Amount { get; set; }

    public Guid? CurrencyId { get; set; }
}
