using Finance.Application.Dtos.Banks;
using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.Funds;

public record FundDto : Dto<Guid>
{
    public BankDto Bank { get; set; } = default!;
    public CurrencyDto Currency { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime TimeStamp { get; set; }
    public Money Amount { get; set; } = 0m;
    public bool DailyUse { get; set; }
}
