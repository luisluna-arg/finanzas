using Finance.Application.Commands.Incomes.Owners.Base;

namespace Finance.Application.Services.Requests.Incomes;

public class UpdateIncomeSagaRequest(Guid incomeId, Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount)
    : UpsertIncomeSagaRequestBase(bankId, currencyId, timeStamp, amount)
{
    public Guid IncomeId { get; } = incomeId;
}
