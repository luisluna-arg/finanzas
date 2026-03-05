using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Commands.Incomes.Owners.Base;

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
