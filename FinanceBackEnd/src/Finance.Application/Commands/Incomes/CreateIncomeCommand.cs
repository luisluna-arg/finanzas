using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.Incomes;

public class CreateIncomeCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<Income>>
{
    public virtual Guid BankId { get; set; }
    public virtual Guid CurrencyId { get; set; }
    required public DateTime? TimeStamp { get; set; }
    required public Money Amount { get; set; }
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public class CreateIncomeCommandHandler(
    FinanceDbContext db,
    IRepository<Bank, Guid> bankRepository,
    IRepository<Currency, Guid> currencyRepository,
    IRepository<Income, Guid> incomeRepository)
    : BaseCommandHandler<CreateIncomeCommand, Income>(db)
{
    public override async Task<DataResult<Income>> ExecuteAsync(CreateIncomeCommand command, CancellationToken cancellationToken)
    {
        Bank? bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        Currency? currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var newIncome = new Income()
        {
            Bank = bank,
            Currency = currency,
            Amount = command.Amount,
            TimeStamp = command.TimeStamp ?? DateTime.UtcNow,
            Deactivated = false
        };

        await incomeRepository.AddAsync(newIncome, cancellationToken);

        return DataResult<Income>.Success(newIncome);
    }
}
