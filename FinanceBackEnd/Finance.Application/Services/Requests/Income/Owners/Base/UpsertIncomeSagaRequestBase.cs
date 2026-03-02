using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.Incomes.Owners.Base;

public abstract class UpsertIncomeSagaRequestBase : CreateIncomeCommand, ISagaRequest
{
    protected UpsertIncomeSagaRequestBase(Guid bankId, Guid currencyId, DateTime timeStamp, decimal amount)
        : base()
    {
        BankId = bankId;
        CurrencyId = currencyId;
        TimeStamp = timeStamp;
        Amount = amount;
    }
}
