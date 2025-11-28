using Finance.Application.Commands.Funds.Owners.Base;

namespace Finance.Application.Services.Requests.Funds;

public class CreateFundSagaRequest(Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount, bool dailyUse)
    : UpsertFundSagaRequestBase(bankId, currencyId, timeStamp, amount, dailyUse);
