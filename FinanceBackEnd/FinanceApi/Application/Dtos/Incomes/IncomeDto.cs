using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Application.Dtos.Currencies;
using FinanceApi.Core.SpecialTypes;

namespace FinanceApi.Application.Dtos.Incomes;

public record IncomeDto : Dto<Guid>
{
    public BankDto Bank { get; set; }

    public CurrencyDto Currency { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime TimeStamp { get; set; }

    public Money Amount { get; set; } = 0m;
}
