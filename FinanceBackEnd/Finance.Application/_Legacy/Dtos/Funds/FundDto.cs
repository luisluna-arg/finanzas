using Finance.Application.Legacy.Dtos.Banks;
using Finance.Application.Legacy.Dtos.Base;
using Finance.Application.Legacy.Dtos.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Legacy.Dtos.Funds;

public record FundDto : Dto<Guid>
{
    public BankDto Bank { get; set; } = default!;
    public CurrencyDto Currency { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime TimeStamp { get; set; }
    public Money Amount { get; set; } = 0m;
    public bool DailyUse { get; set; }
}
