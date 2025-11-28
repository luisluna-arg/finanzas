using Finance.Application.Commands.Funds.Owners.Base;

namespace Finance.Application.Services.Requests.Funds;

public class UpdateFundSagaRequest(Guid fundId, Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount, bool dailyUse)
    : UpsertFundSagaRequestBase(bankId, currencyId, timeStamp, amount, dailyUse)
{
    public Guid FundId { get; } = fundId;
}
