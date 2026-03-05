using Finance.Application.Legacy.Commands.Incomes.Owners.Base;

namespace Finance.Application.Legacy.Services.Requests.Incomes;

public class CreateIncomeSagaRequest(Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount)
    : UpsertIncomeSagaRequestBase(bankId, currencyId, timeStamp, amount);
