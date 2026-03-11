using Finance.Domain.SpecialTypes;

namespace Finance.Application.Services.Funds;

public sealed record CreateFundRequest(
    Guid BankId,
    Guid CurrencyId,
    DateTime TimeStamp,
    Money Amount,
    bool DailyUse);

public sealed record UpdateFundRequest(
    Guid Id,
    Guid BankId,
    Guid CurrencyId,
    DateTime TimeStamp,
    Money Amount,
    bool DailyUse);

public sealed record DeleteFundRequest(Guid[] Ids);
