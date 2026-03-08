using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.Incomes;

public class UpdateIncomeCommandHandler(
    FinanceDbContext db,
    IRepository<Bank, Guid> bankRepository,
    IRepository<Currency, Guid> currencyRepository,
    IRepository<Income, Guid> incomeRepository)
    : BaseCommandHandler<UpdateIncomeCommand, Income>(db)
{
    public override async Task<DataResult<Income>> ExecuteAsync(UpdateIncomeCommand command, CancellationToken cancellationToken)
    {
        var income = await incomeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (income == null) throw new Exception("Income not found");

        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        income.Currency = currency;
        income.Bank = bank;
        income.Amount = command.Amount;
        income.TimeStamp = command.TimeStamp ?? DateTime.UtcNow;

        await incomeRepository.UpdateAsync(income, cancellationToken);

        return DataResult<Income>.Success(income);
    }
}

public class UpdateIncomeCommand : ICommand<DataResult<Income>>
{
    required public Guid Id { get; set; }
    public virtual Guid BankId { get; set; }
    public virtual Guid CurrencyId { get; set; }
    required public DateTime? TimeStamp { get; set; }
    required public Money Amount { get; set; }
}
