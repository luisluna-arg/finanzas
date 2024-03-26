using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Application.Dtos.Currencies;
using FinanceApi.Core.SpecialTypes;

namespace FinanceApi.Application.Dtos.Funds;

public record FundDto : Dto<Guid>
{
    public virtual BankDto Bank { get; set; }

    public virtual CurrencyDto Currency { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime TimeStamp { get; set; }

    public Money Amount { get; set; }
}
