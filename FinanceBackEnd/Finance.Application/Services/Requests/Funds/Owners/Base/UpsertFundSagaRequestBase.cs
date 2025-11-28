using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.Funds.Owners.Base;

public abstract class UpsertFundSagaRequestBase : CreateFundCommand, ISagaRequest
{
    protected UpsertFundSagaRequestBase(Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount, bool dailyUse)
        : base()
    {
        BankId = bankId;
        CurrencyId = currencyId;
        TimeStamp = timeStamp;
        Amount = amount;
        DailyUse = dailyUse;
    }
}
