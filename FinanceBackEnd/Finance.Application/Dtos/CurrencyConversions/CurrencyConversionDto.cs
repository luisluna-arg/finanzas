namespace Finance.Application.Dtos.CurrencyConversions;

public record CurrencyConversionDto() : Dto<Guid>
{
    public Guid MovementId { get; set; }

    public decimal Amount { get; set; }

    public Guid? CurrencyId { get; set; }
}
