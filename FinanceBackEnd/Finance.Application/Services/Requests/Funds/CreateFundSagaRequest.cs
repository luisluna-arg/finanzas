using Finance.Application.Commands.Funds;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Funds;

public class CreateFundSagaRequest : CreateFundCommand, ISagaRequest
{
    public CreateFundSagaRequest(Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount, bool dailyUse)
        : base()
    {
        BankId = bankId;
        CurrencyId = currencyId;
        TimeStamp = timeStamp;
        Amount = amount;
        DailyUse = dailyUse;
    }
}
