using Finance.Application.Commands.Funds;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Funds;

public class UpdateFundSagaRequest : UpdateFundCommand, ISagaRequest
{
    public Guid FundId { get; }

    public UpdateFundSagaRequest(Guid userId, Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount, bool dailyUse)
        : base()
    {
        FundId = userId;
        BankId = bankId;
        CurrencyId = currencyId;
        TimeStamp = timeStamp;
        Amount = amount;
        DailyUse = dailyUse;
    }
}
