using Finance.Application.Legacy.Dtos.Banks;
using Finance.Application.Legacy.Dtos.Base;
using Finance.Application.Legacy.Dtos.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Legacy.Dtos.Incomes;

public record IncomeDto : Dto<Guid>
{
    public IncomeDto() { }

    public BankDto Bank { get; set; } = default!;
    public CurrencyDto Currency { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public Money Amount { get; set; } = 0m;
}
