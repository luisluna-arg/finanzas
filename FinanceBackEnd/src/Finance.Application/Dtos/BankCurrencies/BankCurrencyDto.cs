using Finance.Application.Dtos.Banks;
using Finance.Application.Dtos.Currencies;

namespace Finance.Application.Dtos.BankCurrencies;

public record BankCurrencyDto
{
    public Guid BankId { get; set; }
    public Guid CurrencyId { get; set; }
    public BankDto Bank { get; set; } = default!;
    public CurrencyDto Currency { get; set; } = default!;
    public bool DailyUse { get; set; }
    public bool Deactivated { get; set; }
}
