using Finance.Application.Dtos.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.CurrencyConversions;

public record CurrencyConversionDto : Dto<Guid>
{
    public CurrencyConversionDto() { }

    public Guid MovementId { get; set; } = Guid.Empty;
    public Money Amount { get; set; } = 0M;
    public Guid? CurrencyId { get; set; } = null;
}
