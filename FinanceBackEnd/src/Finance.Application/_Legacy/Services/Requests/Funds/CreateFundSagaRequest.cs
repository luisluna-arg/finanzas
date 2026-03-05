using Finance.Application.Legacy.Commands.Funds.Owners.Base;

namespace Finance.Application.Legacy.Services.Requests.Funds;

public class CreateFundSagaRequest(Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount, bool dailyUse)
    : UpsertFundSagaRequestBase(bankId, currencyId, timeStamp, amount, dailyUse);
