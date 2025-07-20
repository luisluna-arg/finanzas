using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.CurrencyConversions;

public record CurrencyConversionDto : Dto<Guid>
{
    public Guid MovementId { get; set; } = Guid.Empty;

    public decimal Amount { get; set; } = 0M;

    public Guid? CurrencyId { get; set; } = null;

    public CurrencyConversionDto()
    {
    }
}
