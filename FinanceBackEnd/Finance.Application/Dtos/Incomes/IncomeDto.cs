using Finance.Application.Dtos.Banks;
using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.Incomes;

public record IncomeDto : Dto<Guid>
{
    public BankDto Bank { get; set; } = default!;

    public CurrencyDto Currency { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    public Money Amount { get; set; } = 0m;
    
    public IncomeDto()
    {
    }
}
