using Finance.Domain.SpecialTypes;

namespace Finance.Application.Services.Incomes;

public sealed record CreateIncomeRequest(
    Guid BankId,
    Guid CurrencyId,
    DateTime TimeStamp,
    Money Amount);

public sealed record UpdateIncomeRequest(
    Guid Id,
    Guid BankId,
    Guid CurrencyId,
    DateTime TimeStamp,
    Money Amount);

public sealed record DeleteIncomeRequest(Guid[] Ids);
